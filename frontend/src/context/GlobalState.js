import React, { useState } from 'react'
import GlobalContext from './GlobalContext'

const GlobalState = props => {
  const [authed, setAuthed] = useState(false)
  const [session, setSession] = useState('')
  const [email, setEmail] = useState()
  const [userGlobal, setUser] = useState()
  const [order, setOrder] = useState({})
  const [orders, setOrders] = useState([])
  const [images, setImages] = useState([])
  const [isBrokerView, setIsBrokerView] = useState(false)

  const handleAuthed = value => setAuthed(value)
  const handleEmail = value => setEmail(value)
  const handleUser = value => setUser(value)
  const handleOrder = value => setOrder(value)
  const handleOrders = value => setOrders(value)
  const handleSession = value => setSession(value)
  const handleImages = arr => setImages(arr)
  const handleIsBrokerView = value => setIsBrokerView(value)

  return (
    <GlobalContext.Provider
      value={{
        authed,
        handleAuthed,
        email,
        handleEmail,
        userGlobal,
        handleUser,
        order,
        handleOrder,
        orders,
        handleOrders,
        session,
        handleSession,
        images,
        handleImages,
        isBrokerView,
        handleIsBrokerView
      }}
    >
      {props.children}
    </GlobalContext.Provider>
  )
}

export default GlobalState
