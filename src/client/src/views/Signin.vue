<template>
    <div>
        <h1 class="text-center">Sign In</h1>
        <b-form @submit.prevent class="border login-form">
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

            <b-button type="submit" variant="primary" class="mr-3" v-on:click="login">Submit</b-button>
            <b-button type="reset" variant="secondary" v-on:click="resetForm" >Reset</b-button>
        </b-form>
    </div>
</template>

<script>
import httpClient from "../helpers/http-client";
import {mapActions} from "vuex";

export default {
    name: "Login",
    data() {
        return {
            newUser: {
                username: null,
                password: null,
            },
        };
    },
    methods: {
        ...mapActions(["loginStore"]),
        resetForm() {
            this.newUser = {
                username: null,
                password: null,
            };
        },
        login() {
            httpClient.post("/users/auth", this.newUser)
            .then((response) => {
                this.loginStore(response.token);
                this.$router.push({name: "Home"});
            })
            .catch(() => {});
        }
    },
};
</script>

<style scoped>
.login-form {
    max-width: 500px;
    margin: auto;
    padding: 2rem;
}
</style>
