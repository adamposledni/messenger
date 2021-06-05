import store from "../store/index";
import router from "../router/index";

async function get(specUrl) {
    return httpRequest("GET", specUrl);
}

async function del(specUrl) {
    return httpRequest("DELETE", specUrl);
}

async function post(specUrl, body) {
    return httpRequest("POST", specUrl, body)
}

async function put(specUrl, body) {
    return httpRequest("PUT", specUrl, body);
}

// general HTTP request
async function httpRequest(method, specUrl, body = null) {
    store.commit("setLoading", true);

    let stringBody = null;
    if (body) {
        stringBody = JSON.stringify(body)
    }

    let request =  {
        method: method,
        mode: "cors",
        headers: {
            "Content-Type": "application/json"
        },
        body: stringBody,
    };

    if (store.state.accessToken) {
        request.headers["Authorization"] = "Bearer " + store.state.accessToken;
    }

    let response = await fetch(store.state.baseApiUrl + specUrl, request)
        .catch(() => {
            raiseError({ error: "Error has occured", detail: null });
        })
        .finally(() => {
            store.commit("setLoading", false);
        });

    switch (response.status) {
        case 200: case 201:
            return response.json();
        case 204:
            return null;
        case 401:
            store.dispatch("logoutStore");
            router.push({name: "SignIn"});
            raiseError({ error: "Unauthorized", detail: null });
            break;
        case 403:
            raiseError({ error: "Forbidden", detail: null });
            break;
        case 404:
            raiseError({ error: "Not found", detail: null });
            break;
        default:
            raiseError(await response.json());
            break;
    }
}

function raiseError(errObj) {
    store.commit("setErrorMsg", errObj);
    throw new Error();
}

// function replacer(key, value) {
//     switch (key) {
//         case "xxx":
//             return +value;
//         default:
//             return value;
//     }
// }

export default {
    get, post, put, del
}