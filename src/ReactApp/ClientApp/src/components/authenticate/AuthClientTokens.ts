import { User } from "oidc-client"

export type AuthClientTokens = 
Pick<User, 'id_token' | 'access_token' | 'refresh_token'>