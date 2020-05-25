import React, { useContext, useState, useEffect } from 'react'
import GlobalContext from '../../../context/GlobalContext'
import FormShape from './FormShape'
import { Container, Card } from 'react-bootstrap'
import SwitchButton from './SwitchButton'
import UserDetailRow from './UserDetailRow'
import lang from '../../../language/SWE/UserDetails.json'

const UserData = ({
  setNewChanges,
  setCompany,
  setOrgNr,
  setAddress,
  setPostalCode,
  setCity,
  setCellPhoneNumber,
  setFirstName,
  setLastName,
  setPostalCodeArea,
  setAvailable,
  available
}) => {
  const context = useContext(GlobalContext)
  const [editCompany, setEditCompany] = useState(false)
  const [editOrgNr, setEditOrgNr] = useState(false)
  const [editAddress, setEditAddress] = useState(false)
  const [editPostalCode, setEditPostalCode] = useState(false)
  const [editCity, setEditCity] = useState(false)
  const [editCellPhoneNumber, setEditCellPhoneNumber] = useState(false)
  const [editFirstName, setEditFirstName] = useState(false)
  const [editLastName, setEditLastName] = useState(false)
  const [editPostalCodeArea, setEditPostalCodeArea] = useState(false)

  useEffect(() => {
    setEditCompany(false)
    setEditOrgNr(false)
    setEditAddress(false)
    setEditPostalCode(false)
    setEditCity(false)
    setEditCellPhoneNumber(false)
    setEditFirstName(false)
    setEditLastName(false)
    setEditPostalCodeArea(false)
  }, [])

  return (
    <Container>
      <Card border='secondary' style={{ width: '40rem' }}>
        <Card.Header>{lang.register.aboutcompany}</Card.Header>
        <Card.Body className='account-manager-card-body'>
          {editCompany ? (
            <FormShape
              header={lang.company}
              placeholder={context.userGlobal.company}
              setState={setCompany}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.company}
                value={context.userGlobal.company}
                setStateClick={setEditCompany}
              />
            </>
          )}
          {editOrgNr ? (
            <FormShape
              header={lang.orgnr}
              placeholder={context.userGlobal.orgNr}
              setState={setOrgNr}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.orgnr}
                value={context.userGlobal.orgNr}
                setStateClick={setEditOrgNr}
              />
            </>
          )}
          {editAddress ? (
            <FormShape
              header={lang.address}
              placeholder={context.userGlobal.address}
              setState={setAddress}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.address}
                value={context.userGlobal.address}
                setStateClick={setEditAddress}
              />
            </>
          )}
          {editPostalCode ? (
            <FormShape
              header={lang.postalcode}
              placeholder={context.userGlobal.postalCode}
              setState={setPostalCode}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.postalcode}
                value={context.userGlobal.postalCode}
                setStateClick={setEditPostalCode}
              />
            </>
          )}
          {editCity ? (
            <FormShape
              header={lang.city}
              placeholder={context.userGlobal.city}
              setState={setCity}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.city}
                value={context.userGlobal.city}
                setStateClick={setEditCity}
              />
            </>
          )}
          {editFirstName ? (
            <FormShape
              header={lang.firstname}
              placeholder={context.userGlobal.firstName}
              setState={setFirstName}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.firstname}
                value={context.userGlobal.firstName}
                setStateClick={setEditFirstName}
              />
            </>
          )}
          {editLastName ? (
            <FormShape
              header={lang.lastname}
              placeholder={context.userGlobal.lastName}
              setState={setLastName}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.lastname}
                value={context.userGlobal.lastName}
                setStateClick={setEditLastName}
              />
            </>
          )}
          {editCellPhoneNumber ? (
            <FormShape
              header={lang.cellphonenumber}
              placeholder={context.userGlobal.cellPhoneNumber}
              setState={setCellPhoneNumber}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.cellphonenumber}
                value={context.userGlobal.cellPhoneNumber}
                setStateClick={setEditCellPhoneNumber}
              />
            </>
          )}
          {editPostalCodeArea ? (
            <FormShape
              header={lang.postalcodearea}
              placeholder={context.userGlobal.postalCodeArea}
              setState={setPostalCodeArea}
              setNewChanges={setNewChanges}
            />
          ) : (
            <>
              <UserDetailRow
                header={lang.postalcodearea}
                value={context.userGlobal.postalCodeArea}
                setStateClick={setEditPostalCodeArea}
              />
            </>
          )}
          <SwitchButton
            setState={setAvailable}
            isChecked={available}
            setNewChanges={setNewChanges}
          />
        </Card.Body>
      </Card>
    </Container>
  )
}

export default UserData
