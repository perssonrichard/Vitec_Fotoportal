import React, { useEffect, useContext } from 'react'
import { Route, useHistory, Switch } from 'react-router-dom'
import axios from 'axios'
import GlobalContext from '../../context/GlobalContext'
import { getSessionCookie } from '../../helpers/CookieHandler'

import Menu from './Menu'
import AccountManager from './UserDetails/AccountManager'
import PasswordManager from './UserDetails/PasswordManager'
import RemoveAccount from './UserDetails/RemoveAccount'
import OrderDetails from './OrderDetails'
import ViewOrders from './ViewOrders'
import Login from './Login'
import Register from './Register'
import NotFound from './NotFound'
import URL from '../../models/URL'

const PhotographerView = () => {
  const context = useContext(GlobalContext)
  const history = useHistory()

  useEffect(() => {
    const source = axios.CancelToken.source()

    if (context.session === '' && getSessionCookie() !== undefined) {
      context.handleSession(getSessionCookie())
    }

    if (context.authed === false && context.session !== '') {
      axios
        .get(URL.SERVER_LOGIN_URL, {
          headers: { Authorization: `Bearer ${getSessionCookie()}` },
          cancelToken: source.token
        })
        .then(function (res) {
          if (res.status === 200) {
            context.handleEmail(res.data)
            context.handleAuthed(true)
          }
        })
        .catch(function (err) {
          // If not cancelled by source.cancel()
          if (err.message !== undefined) history.push('/login')
        })
    }

    if (
      context.email !== undefined &&
      context.authed &&
      context.userGlobal === undefined
    ) {
      axios({
        method: 'GET',
        url: URL.SERVER_GET_USER_URL + context.email,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getSessionCookie()}`
        },
        cancelToken: source.token
      })
        .then(res => {
          context.handleUser(res.data.photographer)
        })
        .catch(err => {
          if (axios.isCancel()) console.log(err)
        })
    }

    // Load order data if logged in and not loaded
    if (
      context.authed &&
      context.session !== '' &&
      context.orders.length === 0
    ) {
      // getOrders
      axios({
        method: 'GET',
        url: URL.SERVER_GET_ORDER_URL,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getSessionCookie()}`
        },
        cancelToken: source.token
      })
        .then(function (response) {
          if (response.status === 200 && response.data.length !== 0) {
            context.handleOrders(response.data.map(file => Object.assign(file)))
          }
        })
        .catch(function (error) {
          console.log(error)
        })
    }

    return () => {
      return source.cancel()
    }
  }, [context, history])

  return (
    <>
      <Menu />
      <Switch>
        <Route
          exact
          path='/'
          component={
            context.session !== '' && context.authed ? ViewOrders : Login
          }
        />
        <Route
          path='/login'
          component={
            context.session !== '' && context.authed ? ViewOrders : Login
          }
        />
        <Route
          path='/register'
          component={
            context.session !== '' && context.authed ? ViewOrders : Register
          }
        />
        <Route
          path='/account'
          component={
            context.session !== '' && context.authed ? AccountManager : Login
          }
        />
        <Route
          path='/pwhelper'
          component={
            context.session !== '' && context.authed ? PasswordManager : Login
          }
        />
        <Route
          path='/removeaccount'
          component={
            context.session !== '' && context.authed ? RemoveAccount : Login
          }
        />
        <Route
          path='/order/:id'
          component={
            context.session !== '' && context.authed ? OrderDetails : Login
          }
        />
        <Route
          path='/orders'
          component={
            context.session !== '' && context.authed ? ViewOrders : Login
          }
        />

        <Route component={NotFound} />
      </Switch>
    </>
  )
}

export default PhotographerView
