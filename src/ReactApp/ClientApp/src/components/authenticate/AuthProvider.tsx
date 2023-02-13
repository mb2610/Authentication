import React, { useState } from 'react';

import { AuthService } from './AuthService';
import { AuthContext } from './Context';

const AuthProvider: React.FC<{children: React.ReactNode}> = (props) => {
    const [api] = useState(new AuthService());

    return (
        <AuthContext.Provider value={api}>
            {props.children}
        </AuthContext.Provider>
    )
}
export default AuthProvider;