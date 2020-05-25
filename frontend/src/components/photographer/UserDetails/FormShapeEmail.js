import React from 'react'
import { Form } from 'react-bootstrap'
import InputGroup from 'react-bootstrap/InputGroup'

const FormShapeEmail = ({ placeholder, setState, setNewChanges }) => {
  return (
    <InputGroup className='mb-3'>
      <InputGroup.Prepend>
        <InputGroup.Text id='basic-addon1'>@</InputGroup.Text>
      </InputGroup.Prepend>
      <Form.Control
        onChange={event => setState(event.target.value, setNewChanges(true))}
        type='text'
        placeholder={placeholder}
        required
        aria-describedby='basic-addon1'
      />
    </InputGroup>
  )
}

export default FormShapeEmail
