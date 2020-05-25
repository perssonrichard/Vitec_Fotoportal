import React, { useState, useContext } from 'react'
import { Button, Container, Form, Row, Alert } from 'react-bootstrap'
import { NavLink, useHistory } from 'react-router-dom'
import InputGroup from 'react-bootstrap/InputGroup'
import Card from 'react-bootstrap/Card'
import GlobalContext from '../../context/GlobalContext'
import URL from '../../models/URL'
import axios from 'axios'
import * as cookieHandler from '../../helpers/CookieHandler'
import FlashMessage from 'react-flash-message'
import lang from '../../language/SWE/Login.json'

const Login = () => {
  const context = useContext(GlobalContext)

  const [email, setEmail] = useState()
  const [password, setPassword] = useState()
  const [wrongCredentials, setWrongCredentials] = useState(false)
  const [serverError, setServerError] = useState(false)
  const history = useHistory()

  const currentEmail = event => {
    setEmail(event.target.value)
  }

  const currentPassword = event => {
    setPassword(event.target.value)
  }

  const handleClick = event => {
    event.preventDefault()
    postCredentials()
  }

  const postCredentials = () => {
    axios
      .post(URL.SERVER_LOGIN_URL, {
        email: email,
        password: password
      })
      .then(function (res) {
        if (res.status === 200) {
          context.handleSession(res.data.token)
          cookieHandler.setSessionCookie(res.data.token)
          history.push('/orders')
        }
      })
      .catch(err => {
        if (err) {
          if (err.response !== undefined) {
            if (err.response.status === 400 || err.response.status === 401) {
              setWrongCredentials(true)
            }
          } else {
            setServerError(true)
          }
        }
      })
  }

  const renderMessage = () => (
    <FlashMessage duration={5000}>
      <Alert variant='danger'>{lang.wrongCredentialsMessage}</Alert>
    </FlashMessage>
  )

  const renderMessage2 = () => (
    <FlashMessage duration={5000}>
      <Alert variant='danger'>{lang.serverError}</Alert>
    </FlashMessage>
  )

  const renderLogin = () => (
    <Container>
      {wrongCredentials && renderMessage()}
      {serverError && renderMessage2()}
      <Row className='justify-content-md-center'>
        <Card border='secondary' style={{ width: '25rem' }}>
          <Card.Header>{lang.title}</Card.Header>
          <Card.Body>
            <Card.Title>{lang.welcomeMessage}</Card.Title>
            <Card.Subtitle className='mb-2 text-muted'>
              {lang.login}
            </Card.Subtitle>
            <Form>
              <InputGroup className='mb-3'>
                <InputGroup.Prepend>
                  <InputGroup.Text id='basic-addon1'>@</InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                  onChange={currentEmail}
                  type='email'
                  placeholder={lang.enterEmail}
                  aria-describedby='basic-addon1'
                />
              </InputGroup>
              <Form.Group controlId='formBasicPassword'>
                <Form.Control
                  onChange={currentPassword}
                  type='password'
                  placeholder={lang.enterPassword}
                />
              </Form.Group>
              <Row style={{ justifyContent: 'center' }}>
                <Button
                  style={{ width: 110, marginTop: -3, marginBottom: 7 }}
                  onClick={handleClick}
                  variant='primary'
                  type='submit'
                >
                  {lang.login}
                </Button>
              </Row>
              <Row style={{ justifyContent: 'center' }}>
                <NavLink to='/register'>{lang.clickToRegister}</NavLink>
              </Row>
            </Form>
          </Card.Body>
        </Card>
      </Row>
    </Container>
  )
  return renderLogin()
}

export default Login
