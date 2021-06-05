<template>
    <div id="app">
        <b-navbar toggleable="lg" type="dark" variant="dark">
            <b-navbar-brand :to="{ name: 'Home' }">Messenger</b-navbar-brand>
            <b-navbar-nav>
                <b-nav-item :to="{ name: 'SignUp' }" v-if="!isUserLogged">Sign Up</b-nav-item>
            </b-navbar-nav>

            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav class="ml-auto">
                    <b-nav-item v-on:click="logout" v-if="isUserLogged"
                        ><b-icon icon="power"></b-icon
                    ></b-nav-item>
                    <b-nav-item :to="{ name: 'SignIn' }" v-else
                        ><b-icon icon="box-arrow-right"></b-icon
                    ></b-nav-item>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
        <b-overlay :show="loading" variant="transparent" rounded="sm" no-wrap> </b-overlay>
        <b-container class="mt-3">
            <router-view />
        </b-container>
    </div>
</template>

<script>
import { mapActions, mapGetters, mapState } from "vuex";
import Vue from "vue";

export default {
    name: "App",
    computed: {
        ...mapState(["loading", "errorMsg", "accessToken"]),
        ...mapGetters(["isUserLogged"]),
    },
    methods: {
        ...mapActions(["logoutStore"]),
        logout() {
            this.logoutStore();
            this.$router.push({name: "SignIn"});
        }
    },
    watch: {
        errorMsg: function(errorMsg) {
            let title;
            let detail;

            if (!errorMsg.detail) {
                title = "Chyba";
                detail = errorMsg.error;
            } else {
                title = errorMsg.error;
                detail = errorMsg.detail;
            }

            this.$bvToast.toast(detail, {
                title: title,
                variant: "danger",
                solid: true,
            });
        },
    },
    created () {
        if (this.isUserLogged) {
            Vue.prototype.startSignalR(this.accessToken);
        }
    },
};
</script>

<style scoped></style>
