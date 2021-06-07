<template>
    <div>
        <b-row v-if="isUserLogged">
            <b-col cols="3">
                <ChatList :chats="chats"></ChatList>
            </b-col>
            <b-col cols="9" v-if="currentChat && currentChat.id">
                <Chat :messages="messages"></Chat>
            </b-col>
        </b-row>
    </div>
</template>

<script>
import { mapActions, mapGetters, mapMutations, mapState } from "vuex";

import ChatList from "../components/ChatList";
import Chat from "../components/Chat";

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
        ...mapActions(["loadChats"]),
        receiveMessage(message) {
            if (message.chatId === this.currentChat.id) {
                this.addMessage(message);
            } 
            else {
                var audio = new Audio(require('@/assets/sounds/message.mp3'))
                audio.play();       
                this.$bvToast.toast(message.content, {
                    title: message.author.firstName + " " + message.author.lastName,
                    variant: "info",
                    solid: true,
                    toaster: "b-toaster-bottom-right",
                    autoHideDelay: 500
                });
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
