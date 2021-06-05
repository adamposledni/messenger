<template>
    <div>
        <b-row v-if="isUserLogged">
            <b-col cols="3">
                <ChatList :chats="chats"></ChatList>
            </b-col>
            <b-col cols="9" v-if="currentChat">
                <Chat :messages="messages"></Chat>
            </b-col>
        </b-row>
    </div>
</template>

<script>
import { mapGetters, mapMutations, mapState } from "vuex";

import ChatList from "../components/ChatList";
import Chat from "../components/Chat";
import httpClient from "../helpers/http-client";

export default {
    name: "Home",
    components: {
        ChatList,
        Chat,
    },
    computed: {
        ...mapGetters(["isUserLogged"]),
        ...mapState(["currentChat"]),
    },
    data() {
        return {
            chats: [],
            messages: [],
        };
    },
    methods: {
        ...mapMutations(["setChats", "addMessage"]),
        loadChats() {
            httpClient
                .get("/chats")
                .then((response) => {
                    this.setChats(response);
                })
                .catch(() => {});
        },
        receiveMessage(message) {
            if (message.chatId === this.currentChat.id) {
                this.addMessage(message);
            }
            // else - notification
        },
    },
    created() {
        if (this.isUserLogged) {
            this.loadChats();
            this.$chatHub.$on("receive-message", this.receiveMessage);
        } else {
            this.$router.push({ name: "SignIn" });
        }
    },
    beforeDestroy() {
        this.$chatHub.$off("receive-message", this.receiveMessage);
    },
};
</script>

<style scoped></style>
