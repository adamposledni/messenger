import Vue from 'vue'
import './plugins/bootstrap-vue'
import SignalRPlugin from './plugins/signal-r'
import App from './App.vue'
import router from './router'
import store from './store'
import './assets/styles/style.css';
import './assets/styles/reset.css';

Vue.config.productionTip = false;
Vue.use(SignalRPlugin);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
