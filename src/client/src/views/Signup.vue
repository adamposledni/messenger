<template>
    <div>
        <h1 class="text-center">Sign Up</h1>
        <b-form @submit.prevent @reset.prevent class="border register-form">
            <b-form-group
                label="Username:"
            >
                <b-form-input
                    v-model="newUser.username"
                    type="text"
                    required
                ></b-form-input>
            </b-form-group>

            <b-form-group
                label="Password:"
            >
                <b-form-input
                    v-model="newUser.password"
                    type="password"
                    required
                ></b-form-input>
            </b-form-group>

            <b-form-group label="First name:">
                <b-form-input
                    v-model="newUser.firstName"
                    required
                ></b-form-input>
            </b-form-group>

            <b-form-group label="Last name:">
                <b-form-input
                    v-model="newUser.lastName"
                    required
                ></b-form-input>
            </b-form-group>

            <b-button type="submit" variant="primary" class="mr-3" v-on:click="registerUser">Submit</b-button>
            <b-button type="reset" variant="secondary" v-on:click="resetForm">Reset</b-button>
        </b-form>
    </div>
</template>

<script>
import httpClient from "../helpers/http-client";

export default {
    name: "Register",
    data() {
        return {
            newUser: {
                firstName: null,
                lastName: null,
                username: null,
                password: null,
            },
        };
    },
    methods: {
        resetForm() {
            this.newUser = {
                firstName: null,
                lastName: null,
                username: null,
                password: null,
            };
        },
        registerUser() {
            httpClient.post("/users", this.newUser)
            .then(() => {
                this.$router.push({name: "Home"});
            })
            .catch(() => {});
        }
    },
};
</script>

<style scoped>
.register-form {
    max-width: 500px;
    margin: auto;
    padding: 2rem;
}
</style>
