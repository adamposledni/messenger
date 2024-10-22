﻿using AutoMapper;
using Messenger.Web.Entities;
using Messenger.Web.Helpers;
using Messenger.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Web.Services
{

    public interface IUserService
    {
        Task<UserRes> CreateAsync(UserCreateReq model);
        Task<UserAuthRes> AuthenticateAsync(UserAuthReq model);
        Task<User> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserNameRes>> ListOthersAsync(string userId);
    }

    public class UserService : IUserService
    {
        private IMongoCollection<User> _userCollection;
        private IMapper _mapper;
        private AppSettings _appSettings;

        public UserService(IMongoCollection<User> userCollection, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userCollection = userCollection;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Create new user
        /// </summary>
        public async Task<UserRes> CreateAsync(UserCreateReq model)
        {
            // check if already exists
            bool userExists = await _userCollection.Find(us => us.Username == model.Username).AnyAsync();
            if (userExists)
            {
                throw new AppLogicException("User with this username already exists");
            }

            // hashing and salting password
            byte[] hash, salt;
            HashPassword(model.Password, out hash, out salt);

            User newUser = new()
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            // insert user into db
            await _userCollection.InsertOneAsync(newUser);            

            return _mapper.Map<UserRes>(newUser);
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        public async Task<UserAuthRes> AuthenticateAsync(UserAuthReq model)
        {
            // check if user exists
            var user = await _userCollection
                .Find(us => us.Username == model.Username).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new AppLogicException("Invalid username or password");
            }

            // verify password
            if (!VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new AppLogicException("Invalid username or password");
            }

            // generate token
            string token = GenerateJwt(user);

            return new UserAuthRes(user.Username, token);
        }

        /// <summary>
        /// List all users (except the one who made the request)
        /// </summary>
        public async Task<IEnumerable<UserNameRes>> ListOthersAsync(string userId)
        {
            var users = await _userCollection.Find(us => us.Id != userId).ToListAsync();
            return _mapper.Map<IEnumerable<UserNameRes>>(users);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userCollection
                .Find(us => us.Id == userId)
                .FirstOrDefaultAsync();

            return user;
        }

        /// <summary>
        /// Verify password
        /// </summary>
        private bool VerifyPassword(string password, byte[] dbHash, byte[] dbSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(dbSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != dbHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Generate JWT
        /// </summary>
        private string GenerateJwt(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Hash password
        /// </summary>
        private void HashPassword(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
