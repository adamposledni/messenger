import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
// import store from "../store";

export default {
    install(Vue) {
        const chatHub = new Vue();
        Vue.prototype.$chatHub = chatHub;
        let connection = null;
        let startedPromise = null;
        let manuallyClosed = false;

        Vue.prototype.startSignalR = (jwtToken) => {
            connection = new HubConnectionBuilder()
                .withUrl(
                    "https://localhost:5001/chat-hub",
                    jwtToken ? { accessTokenFactory: () => jwtToken } : null
                )
                .configureLogging(LogLevel.Information)
                .build();

            // receive
            connection.on("ReceiveMessage", (message) => {
                chatHub.$emit("receive-message", message);
                console.log("ReceiveMessage");
                console.log(message);
            });

            function start() {
                startedPromise = connection.start()
                    .catch(() => {
                        return new Promise((resolve, reject) => setTimeout(() => start().then(resolve).catch(reject), 5000))
                    })
                return startedPromise
            }

            connection.onclose(() => {
                if (!manuallyClosed) start()
            });

            // Start everything
            manuallyClosed = false;
            start();
        };

        Vue.prototype.stopSignalR = () => {
            if (!startedPromise) return;
      
            manuallyClosed = true;
            return startedPromise
              .then(() => connection.stop())
              .then(() => { startedPromise = null });
          }

        // send
        chatHub.invoke = (methodName, data) => {
            // if (!connection) {
                
            // }
            return connection.invoke(methodName, data);
        };
    },
};
