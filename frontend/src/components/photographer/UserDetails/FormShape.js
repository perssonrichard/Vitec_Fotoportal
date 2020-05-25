import React from 'react'
import { Form } from 'react-bootstrap'

const FormShape = ({ header, placeholder, setState, setNewChanges }) => {
  return (
    <div>
      <Form.Group className='formBasic'>
        <Form.Label>{header}</Form.Label>
        <Form.Control
          onChange={event => setState(event.target.value, setNewChanges(true))}
          type='text'
          placeholder={placeholder}
          required
        />
      </Form.Group>
    </div>
  )
}

export default FormShape
