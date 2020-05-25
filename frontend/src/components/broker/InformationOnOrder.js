import React from 'react'
import { Card } from 'react-bootstrap'
import lang from '../../language/SWE/BrokerDetails.json'

const InformationOnOrder = ({ order }) => {
  const getOrderStatus = () => {
    switch (order.status) {
      case 0:
        return 'Created'
      case 1:
        return 'InProgress'
      case 4:
        return 'Cancelled'
      case 5:
        return 'Delivered'
      default:
        return 'Unknown'
    }
  }
  return (
    <>
      {order.status === 1 ? (
        <Card.Header>{lang.informationonorder.orderupdated}</Card.Header>
      ) : (
        <Card.Header>{lang.informationonorder.neworder}</Card.Header>
      )}
      <Card.Subtitle id='order-id'>
        {lang.informationonorder.orderid}: {order.orderId}
      </Card.Subtitle>
      <Card.Subtitle>
        {lang.informationonorder.orderstatus}: {getOrderStatus()}
      </Card.Subtitle>
      <Card.Subtitle id='street-address'>
        {lang.informationonorder.address}: {order.address}
      </Card.Subtitle>
      <Card.Subtitle id='city'>
        {lang.informationonorder.city}: {order.city}
      </Card.Subtitle>
      {order.status === 1 ? (
        <Card.Subtitle id='description'>
          {lang.informationonorder.photographerinfosent}: {order.description}
        </Card.Subtitle>
      ) : (
        <></>
      )}
    </>
  )
}

export default InformationOnOrder
