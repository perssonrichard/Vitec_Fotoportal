import React, { useState } from 'react'
import { Form, Row, Col, Alert } from 'react-bootstrap'
import FlashMessage from 'react-flash-message'
import lang from '../../../language/SWE/UserDetails.json'

const UserPassword = ({
  setNewChanges,
  setOldPassword,
  setNewPassword,
  setNewPwRepeat
}) => {
  const [password, setPassword] = useState()
  const [passwordMatch, setPasswordMatch] = useState()
  const [localChange, setLocalChange] = useState(false)

  const passwordVerifier = event => {
    if (password === event) {
      setPasswordMatch(true)
    } else {
      setPasswordMatch(false)
    }
  }

  return (
    <div>
      {!passwordMatch && localChange ? (
        <>
          <FlashMessage duration={5000}>
            <Alert variant='danger'>{lang.message.isnotsame}</Alert>
          </FlashMessage>
        </>
      ) : (
        <></>
      )}
      <Row>
        <Col>
          <Form.Group controlId='formBasic1'>
            <Form.Label>{lang.currentpassword}</Form.Label>
            <Form.Control
              onChange={event =>
                setOldPassword(event.target.value, setNewChanges(true))}
              type='password'
              placeholder={lang.currentpassword}
              required
            />
          </Form.Group>
        </Col>
        <Col>
          <Form.Group controlId='formBasic2'>
            <Form.Label>{lang.newpassword}</Form.Label>
            <Form.Control
              onChange={event =>
                setNewPassword(
                  event.target.value,
                  setPassword(event.target.value),
                  setNewChanges(true),
                  setLocalChange(true))}
              type='password'
              placeholder={lang.newpassword}
              required
            />
          </Form.Group>
        </Col>
        <Col>
          <Form.Group controlId='formBasic3'>
            <Form.Label>{lang.repeatpassword}</Form.Label>
            <Form.Control
              onChange={event =>
                setNewPwRepeat(
                  event.target.value,
                  passwordVerifier(event.target.value))}
              type='password'
              placeholder={lang.verifypassword}
              required
            />
          </Form.Group>
        </Col>
      </Row>
    </div>
  )
}

export default UserPassword
