import Vue from "vue";
import Vuex from "vuex";
import httpClient from "../helpers/http-client";

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        // base API URL (dev)
        baseApiUrl: "https://localhost:5001/api",

        // base API URL (prod)
        // baseApiUrl: "/api",

        // access token
        accessToken: localStorage.getItem("accessToken") || null,
        // loading overlay
        loading: false,
        // error box
        errorMsg: null,

        currentChat: {
            id: null,
            isGroup: null
        },
        messages: [],
        chats: [],
    },
    getters: {
        isUserLogged(state) {
            return !!state.accessToken;
        },
    },
    mutations: {
        setAccessToken(state, value) {
            state.accessToken = value;
        },
        setLoading(state, value) {
            state.loading = value;
        },
        setErrorMsg(state, value) {
            state.errorMsg = value;
        },
        setCurrentChat(state, value) {
            state.currentChat = value;
        },
        setMessages(state, value) {
            state.messages = value;
        },
        addMessage(state, value) {
            state.messages.push(value);
        },
        setChats(state, value) {
            state.chats = value;
        }
    },
    actions: {
        logoutStore({ commit }) {
            localStorage.removeItem("accessToken");
            commit("setAccessToken", null);
            Vue.prototype.stopSignalR();
        },
        loginStore({ commit, state }, accessToken) {
            commit("setAccessToken", accessToken);
            localStorage.setItem("accessToken", accessToken);
            Vue.prototype.startSignalR(state.accessToken);
        },
        switchChat({ commit }, chat) {
            commit("setCurrentChat", chat);
            let specUrl = "/chats/" + chat.id + "/messages";
            httpClient
                .get(specUrl)
                .then((response) => {
                    commit("setMessages", response);
                })
                .catch(() => { });
        }
    },
    modules: {},
});
