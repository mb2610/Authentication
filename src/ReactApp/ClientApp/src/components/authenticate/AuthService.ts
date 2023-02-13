import { Log, User, UserManager, WebStorageStateStore, UserManagerSettings } from "oidc-client";
import { AuthClientTokens } from "./AuthClientTokens";

export class AuthService {
    private userManager: UserManager;
    private currentUser: User | null = null;
    
    constructor() {
        this.userManager = new UserManager({
            ...getClientSettings(),
            userStore: new WebStorageStateStore({ store: window.sessionStorage }),
        })
        Log.logger = console;
        Log.level = Log.INFO;
        this.userManager.events.addUserLoaded((user) => {
            if (window.location.href.indexOf("signin-oidc") !== -1) {
                this.currentUser = user;
                this.navigateToScreen();
            }
        });
        this.userManager.events.addSilentRenewError((e) => {
            console.log("silent renew error", e.message);
        });
        this.userManager.events.addAccessTokenExpired(() => {
            console.log("token expired");
            this.signinSilent();
        });
    }

    signinRedirectCallback = () => {
        this.userManager.signinRedirectCallback().then(() => {
            "";
        });
    };

    signinRedirect = () => {
        localStorage.setItem("redirectUri", window.location.pathname);
        this.userManager.signinRedirect({});
    };

    navigateToScreen = () => {
        window.location.replace("/");
    };

    isAuthenticated = () => {
        const token = sessionStorage.getItem('oidc.user:http://localhost:8080/auth/realms/demo:react-auth');
        if(!token)
            return false;
        const oidcStorage = JSON.parse(token)
        return (!!oidcStorage && !!oidcStorage.access_token)
    };

    async getUser() : Promise<User | null> {
        if(!this.currentUser) {
            let user = await this.userManager.getUser();
            if (!user) {
                user = await this.userManager.signinRedirectCallback();
            }
            this.currentUser = user;

        }
        return this.currentUser;
    }

    getToken = () : AuthClientTokens => {
        const token = sessionStorage.getItem('oidc.user:http://localhost:8080/auth/realms/demo:react-auth');
        if(!token)
            return {
                id_token: '',
                access_token: ''
            }
        const oidcStorage = JSON.parse(token)
        return oidcStorage
    }

    signinSilent = () => {
        this.userManager.signinSilent()
            .then((user) => {
                this.currentUser = user;
                console.log("signed in", user);
            })
            .catch((err) => {
                console.log(err);
            });
    };

    signinSilentCallback = () => {
        this.userManager.signinSilentCallback();
    };

    createSigninRequest = () => {
        return this.userManager.createSigninRequest();
    };

    logout = () => {
        this.userManager.signoutRedirect({
            id_token_hint: localStorage.getItem("id_token")
        });
        this.userManager.clearStaleState();
        this.currentUser = null;
    };

    signoutRedirectCallback = () => {
        this.userManager.signoutRedirectCallback().then(() => {
            localStorage.clear();
            window.location.replace('');
        });
        this.userManager.clearStaleState();
        this.currentUser = null;
    };
}

export function getClientSettings() : UserManagerSettings {
    return {
        authority: 'http://localhost:8080/auth/realms/demo',
        client_id: 'react-auth',
        redirect_uri: 'http://localhost:3000/signin-oidc',
        silent_redirect_uri: 'http://localhost:3000/silentrenew',
        post_logout_redirect_uri: 'http://localhost:3000/logout',
        response_type: 'id_token token',
        scope: 'openid profile roles',
        filterProtocolClaims: true,
        loadUserInfo: true
    }
};