import React from 'react'
import { Button } from 'react-bootstrap'
import lang from '../../language/SWE/BrokerDetails.json'

const ExitBtn = ({ exitBtn }) => {
  const exitBtnClick = () => {
    // TODO this is a bug, it doesnt work
    window.self.close()
  }

  return (
    <Button
      id='exit-button'
      onClick={exitBtnClick}
      variant='danger'
    >
      {lang.buttons.close}
    </Button>
  )
}

export default ExitBtn
