import React from 'react'
import { Form, Row, Col } from 'react-bootstrap'
import EditBtn from './EditBtn'

const UserDetailRow = ({ header, value, setStateClick, setNewChanges }) => {
  return (
    <div>
      <Form.Group controlId='formBasic' style={detailRowStyle}>
        <Row>
          <Col md={6}>
            <Form.Label>{header}</Form.Label>
          </Col>
          <Col>
            <p>{value}</p>
          </Col>
          <Col md={1}>
            <EditBtn setStateClick={setStateClick} setNewChanges={setNewChanges} />
          </Col>
        </Row>
      </Form.Group>
    </div>
  )
}

const detailRowStyle = {
  borderBottom: 'thin dotted grey'
}

export default UserDetailRow
