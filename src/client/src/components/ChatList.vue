<template>
    <div>
        <div
            class="d-flex justify-content-between align-items-center mb-3 pb-2 px-2 border-bottom"
        >
            <b-avatar></b-avatar>
            <b-button class="" variant="light" v-b-modal="'chat-create-modal'">
                <b-icon
                    class="h4 my-auto"
                    icon="chat-left-dots"
                ></b-icon>
            </b-button>

            <ChatCreate></ChatCreate>

        </div>
        <div
            class="d-flex align-items-center rounded p-2 mb-3 cursor-pointer"
            v-bind:class="{ 'chat-active': currentChat.id === chat.id }"
            v-for="chat in chats"
            v-bind:key="chat.id"
            v-on:click="switchChat(chat)"
        >
            <b-avatar class="mr-3"></b-avatar>
            <span class="ml-2 mr-auto">{{ chat.name }}</span>
        </div>
    </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import ChatCreate from "../components/ChatCreate.vue";

export default {
    name: "ChatList",
    components: {
        ChatCreate,
    },
    computed: {
        ...mapState(["chats", "currentChat"]),
    },
    methods: {
        ...mapActions(["switchChat"]),
    },
};
</script>

<style scoped>
.chat-active {
    background-color: #eeeeee;
}
</style>
