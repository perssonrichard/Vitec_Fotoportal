import React from 'react'
import { Form } from 'react-bootstrap'

const FormPassword = ({
  header,
  placeholder1,
  placeholder2,
  setState,
  setNewChanges
}) => {
  return (
    <Form.Group className='formBasic'>
      <Form.Label>{header}</Form.Label>
      <Form.Control
        onChange={event => setState(event.target.value, setNewChanges(true))}
        type='text'
        placeholder={placeholder1}
      />
      <Form.Control
        onChange={event => setState(event.target.value, setNewChanges(true))}
        type='text'
        placeholder={placeholder2}
      />
    </Form.Group>
  )
}

export default FormPassword
