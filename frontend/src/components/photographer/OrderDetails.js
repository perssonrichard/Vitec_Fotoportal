import React, { useContext, useEffect, useState } from 'react'
import { Nav, Container, Card, ListGroup } from 'react-bootstrap'
import { NavLink, useParams } from 'react-router-dom'
import lang from '../../language/SWE/OrderDetails.json'
import axios from 'axios'
import URL from '../../models/URL'
import GlobalContext from '../../context/GlobalContext'
import OrderImages from './OrderImages'
import { getSessionCookie } from '../../helpers/CookieHandler'

const OrderDetails = () => {
  const context = useContext(GlobalContext)
  const { id } = useParams()

  const [order, setOrder] = useState()
  const [brokerInfo, setBrokerInfo] = useState()

  useEffect(() => {
    const order = context.orders.find(i => { return i.orderId === id })
    const source = axios.CancelToken.source()
    setOrder(order)

    if (order) {
      axios({
        method: 'POST',
        url: URL.SERVER_POST_DEPARTMENT_INFO,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getSessionCookie()}`
        },
        data: {
          installationId: order.installationId,
          departmentId: order.deptId
        },
        cancelToken: source.token
      })
        .then(res => {
          if (res.status === 200) {
            setBrokerInfo(prev => ({
              ...prev,
              ...res.data.result
            }))
          }
        })
        .catch(err => console.log(err))

      axios({
        method: 'POST',
        url: URL.SERVER_POST_BROKER_INFO,
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getSessionCookie()}`
        },
        data: {
          employeeId: order.userId,
          installationId: order.installationId
        },
        cancelToken: source.token
      })
        .then(res => {
          if (res.status === 200) {
            setBrokerInfo(prev => ({
              ...prev, ...res.data.result
            }))
          }
        })
        .catch(err => console.log(err))
    }

    return () => {
      return source.cancel()
    }
  }, [context.orders, context.images, id])

  return (
    <Container>
      <Nav className='mr-auto mt-2 mb-2'>
        <NavLink to='/orders'>← {lang.backToOrdersLink}</NavLink>
      </Nav>

      {
        order !== undefined && brokerInfo !== undefined &&
          <>
            <Card className='order-details-card' border='secondary'>
              <Card.Header>
                <Card.Title>{order.address}, {order.city}</Card.Title>
                <small>{lang.orderDate}{order.regDate.slice(0, 10)}</small>
              </Card.Header>
              <Card.Body>
                <Card.Text>{order.description}</Card.Text>

                <div className='broker-info'>
                  <ListGroup>
                    <ListGroup.Item>{lang.company} {brokerInfo.legalName}</ListGroup.Item>
                    <ListGroup.Item>{lang.address} {brokerInfo.postalAddress}</ListGroup.Item>
                    <ListGroup.Item>{lang.name} {brokerInfo.name}</ListGroup.Item>
                    <ListGroup.Item>{lang.email} {brokerInfo.email}</ListGroup.Item>
                    <ListGroup.Item>{lang.phone} {brokerInfo.mobilePhone}</ListGroup.Item>
                  </ListGroup>
                </div>

              </Card.Body>
            </Card>
            <OrderImages orderId={id} />
          </>
      }
    </Container>
  )
}

export default OrderDetails
