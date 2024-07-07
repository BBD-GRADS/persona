import { clientId, cognitoDomain, backendUrl, redirectUri } from './config.js';

const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const code = urlParams.get('code');
if(code)
    {
    localStorage.setItem("accessToken", code);
    window.location.href = "/"
    }


