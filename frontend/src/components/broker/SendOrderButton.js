import React from 'react'
import { Button } from 'react-bootstrap'
import lang from '../../language/SWE/BrokerDetails.json'

const SendOrderButton = ({ updateOrderWithDescrAndPhotographer }) => {
  return (
    <Button onClick={updateOrderWithDescrAndPhotographer} variant='primary'>
      {lang.buttons.send}
    </Button>
  )
}

export default SendOrderButton
