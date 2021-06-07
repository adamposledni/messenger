<template>
    <b-modal id="chat-create-modal">
        <b-form-select
            v-model="selected"
            multiple
            :select-size="4"
        >
            <option
                v-for="option in options"
                :key="option.id"
                :value="option"
            >
                {{ option.firstName }} {{ option.lastName }}
            </option>
        </b-form-select>

        <template #modal-footer="{ ok }">
            <b-button variant="dark" @click="createChat(ok)">
                Create
            </b-button>
        </template>
    </b-modal>
</template>

<script>
import { mapActions } from 'vuex';
import httpClient from "../helpers/http-client";

export default {
    name: "ChatCreate",
    data() {
        return {
            options: [],
            selected: [],
        };
    },
    methods: {
        ...mapActions(["loadChats"]),
        createChat(ok) {
            if (this.selected && this.selected.length > 0) {
                let body = {
                    memberIds: this.selected.map(o => o.id)
                };

                httpClient.post("/chats", body)
                .then(() => {
                    this.loadChats();
                })
                .catch(() => {});                
            }

            ok();
        },
    },
    created() {
        httpClient
            .get("/users")
            .then((response) => {
                this.options = response;
            })
            .catch(() => {});
    },
};
</script>

<style scoped>
</style>
