import React, { useEffect, useState } from 'react';
import { User } from "oidc-client";
import { useAuthOidc } from "../components/authenticate/useAuth";
import { AuthService } from "../components/authenticate/AuthService";

const Secured = () => {
  const [user, setUser] = useState<User | null>(null);

  const ctx = useAuthOidc();
  const token = ctx.getToken();

  useEffect(() => {
    const api = new AuthService()
    api.getUser().then(user => {
      console.log(user);
      setUser(user);
    });
  }, [token.access_token]);

  // const getGivenName = () => {
  //   if(user?.profile) return user.profile.given_name ?? '';
  //   return '';
  // }

  return (
    <div>
      <h1 className="text-black text-4xl">Welcome to the Protected Page.</h1>
      <p>
        <strong>Id Token:</strong>{token.id_token}
      </p>
      <p>
        <strong>Token:</strong>{token.access_token}
      </p>
      {/* <p>
        <strong>User</strong>{getGivenName()}
      </p> */}
    </div>
  );
  };
  
  export default Secured;