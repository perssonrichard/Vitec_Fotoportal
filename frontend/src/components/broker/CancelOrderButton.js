import React, { useState } from 'react'
import { Button, NavLink } from 'react-bootstrap'
import lang from '../../language/SWE/BrokerDetails.json'

const CancelOrderButton = ({ saveOrderToDatabase, order }) => {
  const [message, setMessage] = useState('')

  const verifyCancelOrder = () => {
    setMessage(<WarningMessageWithLinks />)
  }

  const WarningMessageWithLinks = () => {
    return (
      <>
        <div>{lang.varnings.archives}</div>
        <NavLink className='verificationButtonRed' onClick={stopCancelling}>
          {lang.buttons.cancel}
        </NavLink>
        <NavLink className='verificationButtonGreen' onClick={cancelOrder}>
          {' '}
          {lang.buttons.continue}{' '}
        </NavLink>
      </>
    )
  }

  const stopCancelling = () => {
    setMessage('') // Takes away the warning message and buttons
  }

  const cancelOrder = () => {
    order.status = 4
    saveOrderToDatabase()
  }

  return (
    <div>
      <Button onClick={verifyCancelOrder} variant='primary'>
        {lang.buttons.discard} {order.address}
      </Button>
      <div>{message}</div>
    </div>
  )
}

export default CancelOrderButton
