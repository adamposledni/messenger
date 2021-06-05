<template>
    <div class="d-flex flex-column chat-container mx-3">
        <div class="overflow-auto my-2 p-1" style="height: 100%">
            <Message
                v-for="(message, index) in messages"
                :key="index"
                :message="message"
                :showName="showMessageName(message, index)"
            ></Message>
        </div>

        <b-input-group class="mt-auto rounded">
            <b-form-input
                placeholder="New message..."
                v-model="newMessage.content"
                @keyup.enter="sendMessage"
            ></b-form-input>

            <b-input-group-append>
                <b-button v-on:click="sendMessage" variant="dark">
                    <b-icon icon="cursor" aria-hidden="true"></b-icon>
                </b-button>
            </b-input-group-append>
        </b-input-group>
    </div>
</template>

<script>
import Message from "../components/Message";
import { mapState } from "vuex";

export default {
    name: "Chat",
    components: {
        Message,
    },
    data() {
        return {
            newMessage: {
                chatId: null,
                content: null,
            },
        };
    },
    computed: {
        ...mapState(["messages", "currentChat"]),
    },
    methods: {
        sendMessage() {
            if (
                this.newMessage &&
                this.newMessage.content &&
                this.currentChat
            ) {
                this.newMessage.chatId = this.currentChat.id;
                this.$chatHub.invoke("SendMessage", this.newMessage);
                this.newMessage.content = null;
            }
        },
        showMessageName(currMessage, currentIndex) {
            // show only in group chat
            if (!this.currentChat.isGroup) {
                return false;
            }

            // show only for others messages
            if (currMessage.isOwn) {
                return false;
            }

            // always show for fist message in chat (exclude own)
            if (currentIndex == 0) {
                return true;
            }

            // check if previous message was from the same user
            let prevMessage = this.messages[currentIndex-1];
            if (prevMessage.author.id === currMessage.author.id) {
                return false;
            }

            return true;
        }
    },
};
</script>

<style scoped>
.chat-container {
    height: calc(100vh - 7rem);
}
</style>
