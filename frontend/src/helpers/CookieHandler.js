import * as Cookies from 'js-cookie'
const SESSION = 'session'

export const setSessionCookie = session => {
  const halfHour = new Date(new Date().getTime() + 30 * 60 * 1000)
  Cookies.remove(SESSION)
  Cookies.set(SESSION, session, { expires: halfHour })
}

export const getSessionCookie = () => {
  return Cookies.get(SESSION)
}

export const deleteSessionCookie = () => {
  Cookies.remove(SESSION)
}
