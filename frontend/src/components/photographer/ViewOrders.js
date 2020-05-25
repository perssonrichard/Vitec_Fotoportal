import React, { useContext } from 'react'
import { Table, Container } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import GlobalContext from '../../context/GlobalContext'
import lang from '../../language/SWE/ViewOrders.json'

const ViewOrders = () => {
  const context = useContext(GlobalContext)
  const history = useHistory()

  const handleClick = orderId => {
    history.push(`/order/${orderId}`)
  }

  const viewOrders = () => {
    return context.orders.length !== 0 ? (
      context.orders.map((file, index) => {
        switch (file.status) {
          case 0:
            file.status = lang.orderStatus.created
            break

          case 1:
            file.status = lang.orderStatus.inProgress
            break

          case 2:
            file.status = lang.orderStatus.interactionRequired
            break

          case 3:
            file.status = lang.orderStatus.error
            break

          case 4:
            file.status = lang.orderStatus.cancelled
            break

          case 5:
            file.status = lang.orderStatus.delivered
            break

          default:
        }

        if (
          file.status === lang.orderStatus.cancelled ||
          file.status === lang.orderStatus.delivered
        ) {
          return null
        }

        file.regDate = file.regDate.slice(0, 10)

        return (
          <tr
            id={file.orderId}
            key={index}
            onClick={() => {
              handleClick(file.orderId)
            }}
          >
            <td>{file.username}</td>
            <td>{file.address}</td>
            <td>{file.regDate}</td>
            <td>{file.status}</td>
          </tr>
        )
      })
    ) : (
      <tr>
        <td>{lang.noOrdersInProgress}</td>
        <td />
        <td />
        <td />
      </tr>
    )
  }

  const viewArchive = () => {
    return context.orders.length !== 0 ? (
      context.orders.map((file, index) => {
        if (file.status === lang.orderStatus.cancelled) {
          return (
            <tr id={file.orderId} key={index}>
              <td>{file.username}</td>
              <td>{file.address}</td>
              <td>{file.archiveDate}</td>
              <td>{file.status}</td>
            </tr>
          )
        }
        if (file.status === lang.orderStatus.delivered) {
          file.archiveDate = file.regDate.slice(0, 10)

          return (
            <tr id={file.orderId} key={index}>
              <td>{file.username}</td>
              <td>{file.address}</td>
              <td>{file.archiveDate}</td>
              <td>{file.status}</td>
            </tr>
          )
        }
      })
    ) : (
      <tr>
        <td>{lang.noOrdersInProgress}</td>
        <td />
        <td />
        <td />
      </tr>
    )
  }

  return (
    <Container>
      <div className='order-list'>
        <b>{lang.orderHeader}</b>
        <Table hover>
          <thead>
            <tr>
              <th>{lang.employer}</th>
              <th>{lang.address}</th>
              <th>{lang.orderDate}</th>
              <th>{lang.status}</th>
            </tr>
          </thead>
          <tbody>{viewOrders()}</tbody>
        </Table>

        <b>{lang.archiveHeader}</b>
        <Table hover variant='dark'>
          <thead>
            <tr>
              <th>{lang.employer}</th>
              <th>{lang.address}</th>
              <th>{lang.archiveDate}</th>
              <th>{lang.status}</th>
            </tr>
          </thead>
          <tbody>{viewArchive()}</tbody>
        </Table>
      </div>
    </Container>
  )
}

export default ViewOrders
