import React, { useContext, useState } from 'react'
import { Container, Row, Form, Button, Nav, Alert, Spinner } from 'react-bootstrap'
import { useHistory, NavLink } from 'react-router-dom'
import FlashMessage from 'react-flash-message'
import axios from 'axios'
import GlobalContext from '../../../context/GlobalContext'
import { getSessionCookie } from '../../../helpers/CookieHandler'
import UserPassword from './UserPassword'
import lang from '../../../language/SWE/UserDetails.json'
import langOrders from '../../../language/SWE/OrderDetails.json'
import URL from '../../../models/URL'

const PasswordManager = () => {
  const history = useHistory()
  const context = useContext(GlobalContext)
  const [newChanges, setNewChanges] = useState(false)
  const [newPassword, setNewPassword] = useState()
  const [newPwRepeat, setNewPwRepeat] = useState()
  const [oldPassword, setOldPassword] = useState()
  const [wentGood, setWentGood] = useState(false)
  const [wentWrong, setWentWrong] = useState(false)
  const [wentWrong2, setWentWrong2] = useState(false)
  const [wentWrong3, setWentWrong3] = useState(false)
  const [validated, setValidated] = useState(false)

  const handleSubmit = event => {
    event.preventDefault()
    const form = event.currentTarget
    if (form.checkValidity() === false) {
      event.preventDefault()
      event.stopPropagation()
    }
    if (newPassword !== undefined && newPassword === newPwRepeat) {
      putNewData()
      setNewChanges(false)
      history.push('/pwhelper')
      setValidated(true)
    } else {
      setWentWrong3(true)
      setValidated(false)
    }
  }

  const renderLoading = () => {
    return (
      <Container style={{ textAlign: 'center' }}>
        <Spinner className='mt-3' animation='border' />
      </Container>
    )
  }

  const putNewData = () => {
    const config = {
      method: 'put',
      url: URL.SERVER_EDIT_PASSWORD_URL,
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${getSessionCookie()}`
      },
      data: {
        email: context.userGlobal.email,
        oldPassword: oldPassword,
        newPassword: newPassword
      }
    }

    axios(config)
      .then(res => setWentGood(true))
      .catch(function (error) {
        if (error.response.status === 401) {
          setWentWrong2(true)
        } else if (error) {
          setWentWrong(true)
        } else setWentWrong2(true)
      })
  }

  if (context.userGlobal !== undefined) {
    return (
      <div>
        <Nav className='mr-auto mt-2 mb-2'>
          <NavLink to='/orders'>{langOrders.backToOrdersLink}</NavLink>
        </Nav>
        {wentGood ? (
          <>
            <FlashMessage duration={5000}>
              <Alert variant='success'>
                <Alert.Heading>{lang.message.success}</Alert.Heading>
                {lang.message.wentgood}
              </Alert>
            </FlashMessage>
          </>
        ) : wentWrong ? (
          <FlashMessage duration={5000}>
            <Alert variant='danger'>
              <Alert.Heading>{lang.message.varning}</Alert.Heading>
              {lang.message.wrongpassword}
            </Alert>
          </FlashMessage>
        ) : wentWrong2 ? (
          <FlashMessage duration={5000}>
            <Alert variant='danger'>
              <Alert.Heading>{lang.message.varning}</Alert.Heading>
              {lang.message.wentwrong}
            </Alert>
          </FlashMessage>
        ) : wentWrong3 ? (
          <FlashMessage duration={5000}>
            <Alert variant='danger'>{lang.message.isnotsame}</Alert>
          </FlashMessage>
        ) : (
          <></>
        )}
        <Container>
          <Row className='justify-content-md-center'>
            <Form validated={validated} onSubmit={handleSubmit}>
              <h3>{lang.accountmanager.pwheader}</h3>

              <UserPassword
                setNewChanges={setNewChanges}
                setNewPassword={setNewPassword}
                setOldPassword={setOldPassword}
                setNewPwRepeat={setNewPwRepeat}
              />
              {newChanges ? (
                <Button
                  style={{ width: 200, marginTop: -3, marginBottom: 7 }}
                  variant='primary'
                  type='submit'
                >
                  {lang.accountmanager.save}
                </Button>
              ) : (
                <></>
              )}
            </Form>
          </Row>
        </Container>
      </div>
    )
  } else {
    return renderLoading()
  }
}

export default PasswordManager
