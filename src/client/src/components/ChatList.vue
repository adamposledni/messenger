<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 px-2 border-bottom">
            <b-avatar></b-avatar>
            <b-icon class="h4 mb-2 cursor-pointer" icon="chat-left-dots"></b-icon>
            <!-- chat-left-dots -->
        </div>
        <div
            class="d-flex align-items-center rounded p-2 mb-3 cursor-pointer"
            v-bind:class="{'chat-active': (currentChat.id === chat.id)}"
            v-for="chat in chats" v-bind:key="chat.id"
            v-on:click="switchChat(chat)"
        >
            <b-avatar class="mr-3"></b-avatar>
            <span class="ml-2 mr-auto">{{chat.name}}</span>
        </div>
    </div>
</template>

<script>
import {mapState, mapActions} from "vuex";

export default {
    name: "ChatList",
    computed: {
        ...mapState(["chats", "currentChat"])
    },
    methods: {
        ...mapActions(["switchChat"])
    }
};
</script>

<style scoped>
.chat-active {
    background-color: #eeeeee;
}
</style>
