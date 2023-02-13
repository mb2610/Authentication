import React from "react";


import Nav from "./components/Nav";
import AppRouter from "./components/AppRouter";

import './App.css';
import AuthProvider from "./components/authenticate/AuthProvider";

function App() {  
  return (
    <AuthProvider>
      <Nav />
      <AppRouter />
    </AuthProvider>
  );
}

export default App;
