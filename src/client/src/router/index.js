import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import About from "../views/About.vue";
import SignUp from "../views/Signup.vue";
import SignIn from "../views/Signin.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/about",
    name: "About",
    component: About
  },
  {
    path: "/sign-up",
    name: "SignUp",
    component: SignUp
  },
  {
    path: "/sign-in",
    name: "SignIn",
    component: SignIn
  }
];

const router = new VueRouter({
  routes
});

export default router;
