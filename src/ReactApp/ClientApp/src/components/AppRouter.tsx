import React from "react";

import { BrowserRouter, Route, Routes } from "react-router-dom";
import WelcomePage from "../pages/Home";
import SecuredPage from "../pages/Secured";
import { PrivateRoute } from "../helpers/PrivateRoute";
import { Callback } from "./authenticate/Callback";
import { Logout } from "./authenticate/Logout";
import { LogoutCallback } from "./authenticate/LogoutCallback";
import { SilentRenew } from "./authenticate/SilentRenew";

const AppRouter = () => {
    return (
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<WelcomePage />} />
          <Route
            path="/secured"
            element={<PrivateRoute><SecuredPage /></PrivateRoute>}
          />
          <Route path="/signin-oidc" element={<Callback />} />
          <Route path="/logout" element={<Logout />} />
          <Route path="/logout/callback" element={<LogoutCallback />} />
          <Route path="/silentrenew" element={<SilentRenew />} />        
        </Routes>
      </BrowserRouter>
    )
}
export default AppRouter;