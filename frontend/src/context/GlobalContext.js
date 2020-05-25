import React from 'react'

export default React.createContext({
  authed: '',
  handleAuthed: () => {},
  session: '',
  handleSession: () => {},
  email: '',
  handleEmail: () => {},
  userGlobal: {},
  handleUser: () => {},
  order: {},
  handleOrder: () => {},
  orders: [],
  handleOrders: () => {},
  images: [],
  handleImages: () => {},
  isBrokerView: false,
  handleBrokerView: () => {}
})
