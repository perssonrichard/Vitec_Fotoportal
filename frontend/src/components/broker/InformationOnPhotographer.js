import React, { useState, useEffect } from 'react'
import FlashMessage from 'react-flash-message'
import axios from 'axios'
import { Card } from 'react-bootstrap'
import URL from '../../models/URL_BROKER'
import lang from '../../language/SWE/BrokerDetails.json'

const InformationOnPhotographer = ({ photographer, email, jwt }) => {
  const [chosenPhotographer, setPhotographer] = useState({})
  const [message, setMessage] = useState()

  useEffect(() => {
    if (email !== undefined) {
      getPhotographerInfo()
    }
  })

  const getPhotographerInfo = () => {
    axios
      .get(URL.SERVER_BROKER_GET_ONE_PHOTOGRAPHER_URL + '/' + email, {
        headers: {
          Authorization: jwt,
          Accept: 'application/json'
        }
      })
      .then(res => {
        if (res.data.photographer === null) {
          setMessage(lang.varnings.emptyphotograph)
        } else {
          setPhotographer(res.data.photographer)
        }
      })
      .catch(err => {
        console.log(err)
      })
  }

  if (photographer !== undefined) {
    return (
      <div id='information-on-photographer'>
        <section>
          <Card.Title>{photographer.company}</Card.Title>
          <Card.Subtitle>
            {lang.informationonphotographer.orgnr}: {photographer.orgNr}
          </Card.Subtitle>
          <Card.Subtitle>
            {lang.informationonphotographer.contact}: {photographer.firstName}{' '}
            {photographer.lastName}
          </Card.Subtitle>
          <Card.Subtitle>{photographer.adress}</Card.Subtitle>
          <div>
            {photographer.postalCode} {photographer.city}
          </div>
        </section>
        <section>
          <div>
            {lang.informationonphotographer.cellphone}:{' '}
            {photographer.cellPhoneNumber}
          </div>
          <div>
            {lang.informationonphotographer.email}: {photographer.email}
          </div>
        </section>
      </div>
    )
  } else if (chosenPhotographer !== undefined) {
    return (
      <div id='informationOnPhotographer'>
        <FlashMessage duration={5000} persistOnHover>
          <p className='text-danger'>{message}</p>
        </FlashMessage>
        <section>
          <h2>{chosenPhotographer.company}</h2>
          <div>
            {lang.informationonphotographer.orgnr}: {chosenPhotographer.orgNr}
          </div>
          <div>
            {lang.informationonphotographer.contact}:{' '}
            {chosenPhotographer.firstName} {chosenPhotographer.lastName}
          </div>
          <div>{chosenPhotographer.adress}</div>
          <div>
            {chosenPhotographer.postalCode} {chosenPhotographer.city}
          </div>
        </section>
        <section>
          <div>
            {lang.informationonphotographer.cellphone}:{' '}
            {chosenPhotographer.cellPhoneNumber}
          </div>
          <div>
            {lang.informationonphotographer.email}: {chosenPhotographer.email}
          </div>
        </section>
      </div>
    )
  } else {
    return (
      <>
        <FlashMessage duration={5000} persistOnHover>
          <p className='text-danger'>{lang.varnings.emptyphotograph}</p>
        </FlashMessage>
      </>
    )
  }
}

export default InformationOnPhotographer
