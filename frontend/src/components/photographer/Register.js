import React, { useState, useContext } from 'react'
import { Button, Container, Form, Row, Col, Nav } from 'react-bootstrap'
import { NavLink, useHistory } from 'react-router-dom'
import FlashMessage from 'react-flash-message'
import Card from 'react-bootstrap/Card'
import axios from 'axios'
import GlobalContext from '../../context/GlobalContext'
import URL from '../../models/URL'
import User from '../../models/User'
import FormShape from './UserDetails/FormShape'
import FormShapeEmail from './UserDetails/FormShapeEmail'
import lang from '../../language/SWE/UserDetails.json'

const Register = () => {
  const context = useContext(GlobalContext)
  const history = useHistory()
  const [newChanges, setNewChanges] = useState(false)
  const [allFieldsFilled] = useState()
  const [passwordMatch, setPasswordMatch] = useState(true)
  const [company, setCompany] = useState()
  const [orgNr, setOrgNr] = useState()
  const [address, setAddress] = useState()
  const [postalCode, setPostalCode] = useState()
  const [city, setCity] = useState()
  const [cellPhoneNumber, setCellPhoneNumber] = useState()
  const [postalCodeArea, setPostalCodeArea] = useState()
  const [firstName, setFirstName] = useState()
  const [lastName, setLastName] = useState()
  const [email, setEmail] = useState()
  const [password, setPassword] = useState()
  const [validated] = useState(false)

  const passwordVerifier = event => {
    if (password === event.target.value) {
      setPasswordMatch(true)
      setPassword(password)
    } else {
      setPasswordMatch(false)
    }
  }

  const handleSubmit = event => {
    event.preventDefault()

    const form = event.currentTarget
    if (form.checkValidity() === false) {
      event.stopPropagation()
    }

    postCredentials()
  }
  const postCredentials = () => {
    const user = new User(
      email,
      firstName,
      lastName,
      password,
      cellPhoneNumber,
      address,
      company,
      orgNr,
      city,
      postalCode,
      postalCodeArea
    )
    const config = {
      method: 'post',
      url: URL.SERVER_REGISTER_URL,
      headers: { 'Content-Type': 'application/json' },
      data: user
    }
    axios(config)
      .then(function (response) {
        history.push('/login')
      })
      .catch(function (error) {
        console.log(error)
      })
  }

  return (
    <Container>
      <Nav className='mr-auto mt-2 mb-2'>
        <NavLink to='/login'>← {lang.accountmanager.goback}</NavLink>
      </Nav>
      <Row className='justify-content-md-center'>
        {context.globalAuthed ? (
          <h3>{lang.register.titleuserloggedin}</h3>
        ) : (
          <h3>{lang.register.titleregister}</h3>
        )}
      </Row>
      <Row className='justify-content-md-center'>
        <Form validated={validated} onSubmit={handleSubmit}>
          <Card border='secondary' style={{ width: '40rem' }}>
            <Card.Header>{lang.register.aboutcompany}</Card.Header>
            <Card.Body>
              <Row>
                <Col>
                  <FormShape
                    id='formBasic1'
                    header={lang.company}
                    placeholder={lang.company}
                    setState={setCompany}
                    setNewChanges={setNewChanges}
                  />
                </Col>
                <Col>
                  <FormShape
                    id='formBasic2'
                    header={lang.orgnr}
                    placeholder={lang.orgnr}
                    setState={setOrgNr}
                    setNewChanges={setNewChanges}
                  />
                </Col>
              </Row>
              <FormShape
                header={lang.address}
                placeholder={lang.address}
                setState={setAddress}
                setNewChanges={setNewChanges}
              />
              <Row>
                <Col>
                  <FormShape
                    header={lang.postalcode}
                    placeholder={lang.postalcode}
                    setState={setPostalCode}
                    setNewChanges={setNewChanges}
                  />
                </Col>
                <Col>
                  <FormShape
                    header={lang.city}
                    placeholder={lang.city}
                    setState={setCity}
                    setNewChanges={setNewChanges}
                  />
                </Col>
              </Row>
              <FormShape
                header={lang.cellphonenumber}
                placeholder={lang.cellphonenumber}
                setState={setCellPhoneNumber}
                setNewChanges={setNewChanges}
              />
              <FormShape
                header={lang.postalcodearea}
                placeholder={lang.postalcodearea}
                setState={setPostalCodeArea}
                setNewChanges={setNewChanges}
              />
            </Card.Body>
          </Card>

          <Card border='secondary' style={{ width: '40rem' }}>
            <Card.Header>{lang.register.aboutcontactperson}</Card.Header>
            <Card.Body>
              <Row>
                <Col>
                  <FormShape
                    header={lang.firstname}
                    placeholder={lang.firstname}
                    setState={setFirstName}
                    setNewChanges={setNewChanges}
                  />
                </Col>
                <Col>
                  <FormShape
                    header={lang.lastname}
                    placeholder={lang.lastname}
                    setState={setLastName}
                    setNewChanges={setNewChanges}
                  />
                </Col>
              </Row>
            </Card.Body>
          </Card>

          <Card border='secondary' style={{ width: '40rem' }}>
            <Card.Header>{lang.register.logindetails}</Card.Header>
            <Card.Body>
              <FormShapeEmail
                placeholder={lang.email}
                setState={setEmail}
                setNewChanges={setNewChanges}
              />
              <Row>
                <Col>
                  <Form.Group controlId='formBasic11'>
                    <Form.Label>{lang.password}</Form.Label>
                    <Form.Control
                      onChange={event =>
                        setPassword(event.target.value, passwordVerifier(event))
                      }
                      type='password'
                      placeholder={lang.password}
                    />
                  </Form.Group>
                </Col>
                <Col>
                  <Form.Group controlId='formBasic12'>
                    <Form.Label>{lang.verifypassword}</Form.Label>
                    <Form.Control
                      onChange={event => passwordVerifier(event)}
                      type='password'
                      placeholder={lang.verifypassword}
                    />
                  </Form.Group>
                </Col>
              </Row>
              {passwordMatch ? (
                <></>
              ) : (
                <Row>
                  <p className='text-danger'>{lang.register.passwordmessage}</p>
                </Row>
              )}
            </Card.Body>
          </Card>

          <Row className='justify-content-md-center'>
            {allFieldsFilled ? (
              <></>
            ) : (
              <Row className='justify-content-md-center'>
                <FlashMessage duration={5000} persistOnHover>
                  <p className='text-danger'>{lang.register.fillall}</p>
                </FlashMessage>
              </Row>
            )}
            {newChanges ? (
              <Button style={{ width: 110 }} variant='primary' type='submit'>
                {lang.register.register}
              </Button>
            ) : (
              <></>
            )}
          </Row>
        </Form>
      </Row>
    </Container>
  )
}

export default Register
