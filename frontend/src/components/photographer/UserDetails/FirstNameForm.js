import React, { useContext } from 'react'
import { Form } from 'react-bootstrap'
import GlobalContext from '../../../context/GlobalContext'

const FirstNameForm = ({ handleFirstName }) => {
  const context = useContext(GlobalContext)

  return (
    <div>
      <Form.Group controlId='formBasicFirstName'>
        <Form.Label>Förnamn</Form.Label>
        <Form.Control
          onChange={event => handleFirstName(event.target.value)}
          type='text'
          placeholder={context.userGlobal.firstName}
        />
      </Form.Group>
    </div>
  )
}

export default FirstNameForm
