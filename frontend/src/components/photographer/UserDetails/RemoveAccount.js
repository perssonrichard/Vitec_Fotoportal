import React, { useContext, useEffect } from 'react'
import {
  Container,
  Row,
  Form,
  Button,
  Nav,
  Spinner,
  Accordion,
  Card
} from 'react-bootstrap'
import { useHistory, NavLink } from 'react-router-dom'
import axios from 'axios'
import GlobalContext from '../../../context/GlobalContext'
import * as cookieHandler from '../../../helpers/CookieHandler'
import lang from '../../../language/SWE/UserDetails.json'
import langOrders from '../../../language/SWE/OrderDetails.json'
import URL from '../../../models/URL'

const RemoveAccount = () => {
  const history = useHistory()
  const context = useContext(GlobalContext)

  useEffect(() => {}, [])

  const handleClick = event => {
    event.preventDefault()
    const user = getUserDetails()
    deleteData(user)
    handleRemoveAccount()
  }

  const handleRemoveAccount = () => {
    cookieHandler.deleteSessionCookie()
    context.handleAuthed(false)
    context.handleEmail()
    context.handleOrder({})
    context.handleOrders([])
    context.handleSession('')
    context.handleImages()
    context.handleUser()
    history.push('/login')
  }

  const renderLoading = () => {
    return (
      <Container style={{ textAlign: 'center' }}>
        <Spinner className='mt-3' animation='border' />
      </Container>
    )
  }

  const getUserDetails = () => {
    const userObj = context.userGlobal

    return userObj
  }

  const deleteData = user => {
    const config = {
      method: 'delete',
      url: URL.SERVER_REMOVE_ACCOUNT_URL + context.email,
      headers: {
        Authorization: `Bearer ${cookieHandler.getSessionCookie()}`,
        'Content-Type': 'application/json'
      },
      data: user
    }

    axios(config)
      .then()
      .catch(function (error) {
        console.log(error)
      })
  }

  if (context.userGlobal !== undefined) {
    return (
      <>
        <Container>
          <Row className='justify-content-md-center'>
            <Accordion>
              <Card>
                <Card.Header>
                  <Accordion.Toggle as={Button} variant='link' eventKey='0'>
                    {lang.accountmanager.removeaccountheader}
                  </Accordion.Toggle>
                </Card.Header>
                <Accordion.Collapse eventKey='0'>
                  <Card.Body>
                    <Button
                      onClick={handleClick}
                      variant='danger'
                      type='submit'
                    >
                      {lang.accountmanager.remove}
                    </Button>
                  </Card.Body>
                </Accordion.Collapse>
              </Card>
            </Accordion>
          </Row>
        </Container>
      </>
    )
  } else {
    return renderLoading()
  }
}

export default RemoveAccount
