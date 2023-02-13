import { useContext } from 'react'

import { AuthContext } from './Context'

export function useAuthOidc() {
  const ctx = useContext(AuthContext)
  if (!ctx) {
    throw new Error(
      'useAuthOidc hook must be used inside AuthProvider context'
    )
  }

  return {
    ...ctx
  }
}