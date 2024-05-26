import "vuetify/dist/vuetify.min.css";

import { createApp } from "vue";
import { createPinia } from "pinia";
import App from "./App.vue";
import router from "./router";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { aliases, mdi } from "vuetify/iconsets/mdi";
import { md1 } from "vuetify/blueprints";
import axios from "axios";
import { useAuthStore } from "./stores/authStore";

// Axios interceptor to add the bearer token to every request
axios.interceptors.request.use(
  (config) => {
    const authToken = useAuthStore().token;
    if (authToken) {
      config.headers.Authorization = `Bearer ${authToken}`;
    }
    console.log("Token:", authToken);
    console.log("Making request to:", config.url);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

const vuetify = createVuetify({
  blueprint: md1,
  components,
  directives,
  theme: {
    defaultTheme: "dark",
  },
  icons: {
    defaultSet: "mdi",
    aliases,
    sets: {
      mdi,
    },
  },
});

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(vuetify);

app.mount("#app");
