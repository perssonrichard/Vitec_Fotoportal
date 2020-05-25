import React, { useState, useEffect, useContext } from 'react'
import queryString from 'query-string'
import { useLocation } from 'react-router-dom'
import { Container, Alert, Card } from 'react-bootstrap'
import FlashMessage from 'react-flash-message'
import axios from 'axios'
import URL from '../../models/URL_BROKER'
import InformationOnPhotographer from './InformationOnPhotographer'
import InformationOnOrder from './InformationOnOrder'
import GlobalContext from '../../context/GlobalContext'
import DropDownPhotographerList from './DropDownPhotographerList'
import DescriptionForm from './DescriptionForm'
import CancelOrderButton from './CancelOrderButton'
import SendOrderButton from './SendOrderButton'
import Menu from './../photographer/Menu'
import ExitBtn from './ExitBtn'
import lang from '../../language/SWE/BrokerDetails.json'
// import BookingEmail from './BookingEmail'

const BrokerView = () => {
  const location = useLocation()

  const context = useContext(GlobalContext)

  const values = queryString.parse(location.search)

  const jwt = `Bearer ${values.token} `

  const [order, setOrder] = useState()
  const [photographers, setPhotographers] = useState()
  const [chosenPhotographer, setChosenPhotographer] = useState()
  const [description, setDescription] = useState()
  const [broker, setBroker] = useState() // broker is used on line 45
  const [message, setMessage] = useState()
  const [wentGood, setWentGood] = useState()
  const [wentWrong, setWentWrong] = useState()

  const updateOrderWithDescrAndPhotographer = () => {
    order.status = 1
    order.description = description
    order.photographerEmail = chosenPhotographer.email
    saveOrderToDatabase()
    // TODO send email to photogaphers email with SMTP client
    // SMTP-client sends the message
    // send using BookingEmail(order, chosenPhotographer, broker)
  }

  const saveOrderToDatabase = () => {
    axios({
      method: 'PUT',
      url: URL.SERVER_BROKER_PUT_ORDER_URL,
      headers: {
        Authorization: jwt,
        'Content-Type': 'application/json'
      },
      data: order
    })
      .then(res => {
        if (res.status === 200) {
          setWentGood(true)
          setMessage(lang.varnings.successbooking)
        }
      })
      .catch(err => {
        console.log(err.response)
        setWentWrong(true)
        setMessage(lang.varnings.errorbooking)
      })
  }

  useEffect(() => {
    context.handleIsBrokerView(true)

    const source = axios.CancelToken.source()

    const brokerObj = {
      name: '',
      company: '',
      cellPhoneNr: ''
    }
    // get the order
    axios
      .get(URL.SERVER_BROKER_GET_ORDER_URL, {
        headers: {
          Authorization: jwt,
          Accept: 'application/json'
        },
        cancelToken: source.token
      })
      .then(function (res) {
        setOrder(res.data)
        const orderObj = res.data
        // get brokerinfo to include in the email
        axios({
          method: 'POST',
          url: URL.SERVER_BROKER_GET_ESTATE_INFO_URL,
          headers: {
            Authorization: jwt,
            'Content-Type': 'application/json'
          },
          data: orderObj,
          cancelToken: source.token
        })
          .then(function (res) {
            const employeeId = res.data.result.brokersIdWithRoles[0].employeeId
            axios({
              method: 'POST',
              url: URL.SERVER_BROKER_GET_BROKER_INFO_URL,
              headers: {
                Authorization: jwt,
                'Content-Type': 'application/json'
              },
              data: {
                employeeId: employeeId,
                installationId: orderObj.installationId
              },
              cancelToken: source.token
            })
              .then(function (res) {
                const departmentId = res.data.result.departmentId[0]
                brokerObj.name = res.data.result.name
                brokerObj.cellPhoneNr = res.data.result.mobilePhone
                axios({
                  method: 'POST',
                  url: URL.SERVER_BROKER_GET_DEPARTMENT_INFO,
                  headers: {
                    Authorization: jwt,
                    'Content-Type': 'application/json'
                  },
                  data: {
                    departmentId: departmentId,
                    installationId: orderObj.installationId
                  },
                  cancelToken: source.token
                })
                  .then(function (res) {
                    brokerObj.company = res.data.result.name
                    setBroker(brokerObj)
                  })
                  .catch(err => console.log(err.response))
              })
              .catch(err => console.log(err.response))
          })
          .catch(err => console.log(err.response))
      })
      .catch(err => console.log(err.response))

    // get all photographers
    // TODO should be updated to only get photographers selected by the admin
    axios
      .get(URL.SERVER_BROKER_GET_PHOTOGRAPHERS_URL, {
        headers: {
          Authorization: jwt
        },
        cancelToken: source.token
      })
      .then(function (res) {
        setPhotographers(res.data.data)
      })
      .catch(err => console.log(err))

    return () => {
      return source.cancel()
    }
  }, [])

  return (
    <>
      <Menu />
      {photographers !== undefined && order !== undefined ? (
        <Container>
          <Card id='broker-view-card'>
            {order.status === 0 ? (
              <>
                <InformationOnOrder order={order} />
                <DropDownPhotographerList
                  photographers={photographers}
                  setChosenPhotographer={setChosenPhotographer}
                />
                <DescriptionForm setDescription={setDescription} />
                <InformationOnPhotographer
                  photographer={chosenPhotographer}
                  jwt={jwt}
                />
                <div className='broker-buttons'>
                  <SendOrderButton
                    updateOrderWithDescrAndPhotographer={updateOrderWithDescrAndPhotographer}
                  />
                  <CancelOrderButton
                    saveOrderToDatabase={saveOrderToDatabase}
                    order={order}
                  >
                    {lang.buttons.discard}
                  </CancelOrderButton>
                </div>
              </>
            ) : (
              <>
                <InformationOnOrder order={order} />
                <InformationOnPhotographer email={order.photographerEmail} />
              </>
            )}
            {wentGood ? (
              <>
                <FlashMessage duration={20000}>
                  <Alert variant='success'>
                    <Alert.Heading>{lang.varnings.successheader}</Alert.Heading>
                    {message}
                  </Alert>
                </FlashMessage>
              </>
            ) : wentWrong ? (
              <FlashMessage duration={10000}>
                <Alert variant='danger'>
                  <Alert.Heading>{lang.varnings.errorheader}</Alert.Heading>
                  {message}
                </Alert>
              </FlashMessage>
            ) : (
              <></>
            )}
          </Card>
        </Container>
      ) : (
        <></>
      )}
    </>
  )
}

export default BrokerView
