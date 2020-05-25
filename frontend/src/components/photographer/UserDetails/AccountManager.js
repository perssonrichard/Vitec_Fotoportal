import React, { useContext, useState } from 'react'
import {
  Container,
  Row,
  Form,
  Button,
  Nav,
  Alert,
  Spinner
} from 'react-bootstrap'
import FlashMessage from 'react-flash-message'
import { useHistory, NavLink } from 'react-router-dom'
import axios from 'axios'
import { getSessionCookie } from '../../../helpers/CookieHandler'
import GlobalContext from '../../../context/GlobalContext'
import UserData from './UserData'
import lang from '../../../language/SWE/UserDetails.json'
import langOrders from '../../../language/SWE/OrderDetails.json'
import URL from '../../../models/URL'
import RemoveAccount from './RemoveAccount'

const AccountManager = () => {
  const context = useContext(GlobalContext)
  const history = useHistory()

  const [newChanges, setNewChanges] = useState(false)
  const [company, setCompany] = useState()
  const [orgNr, setOrgNr] = useState()
  const [address, setAddress] = useState()
  const [postalCode, setPostalCode] = useState()
  const [city, setCity] = useState()
  const [cellPhoneNumber, setCellPhoneNumber] = useState()
  const [firstName, setFirstName] = useState()
  const [lastName, setLastName] = useState()
  const [postalCodeArea, setPostalCodeArea] = useState()
  const [available, setAvailable] = useState()
  const [errorMessage, setMessage] = useState(false)

  const handleClick = event => {
    event.preventDefault()
    const userData = editedUser()
    putNewData(userData)
    setNewChanges(false)
  }

  const editedUser = () => {
    const userObj = context.userGlobal

    if (company !== undefined) userObj.company = company
    if (orgNr !== undefined) userObj.orgNr = orgNr
    if (address !== undefined) userObj.address = address
    if (postalCode !== undefined) userObj.postalCode = postalCode
    if (city !== undefined) userObj.city = city
    if (firstName !== undefined) userObj.firstName = firstName
    if (lastName !== undefined) userObj.lastName = lastName
    if (cellPhoneNumber !== undefined) userObj.cellPhoneNumber = cellPhoneNumber
    if (postalCodeArea !== undefined) userObj.postalCodeArea = postalCodeArea
    if (available !== undefined) userObj.available = available

    return userObj
  }

  const putNewData = userData => {
    const config = {
      method: 'put',
      url: URL.SERVER_EDIT_USER_URL + context.email,
      headers: {
        Authorization: `Bearer ${getSessionCookie()}`,
        'Content-Type': 'application/json'
      },
      data: userData
    }

    axios(config)
      .then(function (response) {
        window.location.reload(false)
      })
      .catch(error => {
        setMessage(true)
        console.log(error)
      })
  }

  const handleClick3 = event => {
    event.preventDefault()
    setNewChanges(false)
    // history.push('/orders')
    window.location.reload(false)
  }

  const renderLoading = () => {
    return (
      <Container style={{ textAlign: 'center' }}>
        <Spinner className='mt-3' animation='border' />
      </Container>
    )
  }

  if (context.userGlobal !== undefined) {
    return (
      <Container>
        <Nav className='mr-auto mt-2 mb-2'>
          <NavLink to='/orders'>← {langOrders.backToOrdersLink}</NavLink>
        </Nav>
        <Row className='justify-content-md-center'>
          <Form className='account-manager'>
            {errorMessage ? (
              <FlashMessage duration={5000}>
                <Alert variant='danger'>
                  <Alert.Heading>{lang.message.varning}</Alert.Heading>
                  {lang.message.wentwrong}
                </Alert>
              </FlashMessage>
            ) : (
              <></>
            )}
            <h3>{lang.accountmanager.header}</h3>
            <UserData
              setNewChanges={setNewChanges}
              setCompany={setCompany}
              setOrgNr={setOrgNr}
              setAddress={setAddress}
              setPostalCode={setPostalCode}
              setCity={setCity}
              setCellPhoneNumber={setCellPhoneNumber}
              setFirstName={setFirstName}
              setLastName={setLastName}
              setPostalCodeArea={setPostalCodeArea}
              setAvailable={setAvailable}
              available={available}
            />
            <div className='account-manager-buttons'>
              <Button onClick={handleClick3} variant='primary' type='submit'>
                {lang.accountmanager.cancel}
              </Button>
              {newChanges ? (
                <Button onClick={handleClick} variant='primary' type='submit'>
                  {lang.accountmanager.save}
                </Button>
              ) : (
                <></>
              )}
            </div>
            <RemoveAccount />
          </Form>
        </Row>
      </Container>
    )
  } else {
    return renderLoading()
  }
}

export default AccountManager
