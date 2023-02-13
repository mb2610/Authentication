import { User } from 'oidc-client';
import React from 'react'
import { AuthClientTokens } from './AuthClientTokens';

export const AuthContext = React.createContext({
    signinRedirectCallback: () => {},
    logout: () => {},
    signoutRedirectCallback: () => {},
    isAuthenticated: () => {},
    signinRedirect: () => {},
    signinSilentCallback: () => {},
    createSigninRequest: () => {},
    getToken: () => ({id_token: '',
    access_token: ''} as AuthClientTokens)
})

export const AuthConsumer = AuthContext.Consumer;