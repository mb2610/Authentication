import React from "react";
import { AuthConsumer } from "../components/authenticate/Context"

export const PrivateRoute = ({ children }) => {
    return <AuthConsumer>
        {({ isAuthenticated, signinRedirect }) => {
            if (isAuthenticated()) {
                return <>{children}</>;
            } else {
                signinRedirect();
                return <span>loading</span>;
            }
        }}
    </AuthConsumer>
};