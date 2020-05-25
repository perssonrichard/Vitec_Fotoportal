import React from 'react'
import { Form } from 'react-bootstrap'
import lang from '../../language/SWE/BrokerDetails.json'

const DescriptionForm = ({ setDescription }) => {
  return (
    <Form.Group controlId='formBasic'>
      <Form.Label>{lang.informationonorder.photographerinfo}</Form.Label>
      <Form.Control
        onChange={(event) =>
          setDescription(event.target.value)}
        type='text'
        placeholder='Ange tidpunkt och person att kontakta'
      />
    </Form.Group>
  )
}

export default DescriptionForm
