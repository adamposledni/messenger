<template>
    <div
        class="d-flex"
        v-bind:class="{
            'justify-content-start': !message.isOwn,
            'justify-content-end': message.isOwn,
        }"
    >
        <div class="message">
            <small class="name-text font-weight-light" v-if="showName">{{message.author.firstName + ' ' + message.author.lastName}}</small>
            <div
                class="px-3 py-2 rounded-pill my-1 bg-info text-white"
                v-b-tooltip.hover
                :title="
                    message.author.firstName + ' ' + message.author.lastName
                "
            >
                {{ message.content }}
            </div>
        </div>
    </div>
</template>

<script>
import { mapState } from "vuex";

export default {
    name: "Message",
    props: {
        message: Object,
        showName: Boolean,
    },
    computed: {
        ...mapState(["currentChat"]),
    },
};
</script>

<style scoped>
.message {
    display: inline-block;
}
</style>
