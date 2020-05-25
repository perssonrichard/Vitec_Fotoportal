import React, { useState, useCallback, useEffect, useContext } from 'react'
import { DndProvider } from 'react-dnd'
import { getSessionCookie } from '../../helpers/CookieHandler'
import axios from 'axios'
import HTML5Backend from 'react-dnd-html5-backend'
import update from 'immutability-helper'
import cuid from 'cuid'
import { Modal, Button, Spinner, Container, Alert, Tooltip, OverlayTrigger } from 'react-bootstrap'
import FlashMessage from 'react-flash-message'
import '@fortawesome/fontawesome-free/css/all.css'

import Image from '../../models/Image'
import ImageList from './ImageList'
import GlobalContext from '../../context/GlobalContext'
import URL from '../../models/URL'
import lang from '../../language/SWE/OrderImages.json'

/**
 * Handles images on an order
 */
const OrderImages = ({ orderId }) => {
  const context = useContext(GlobalContext)
  const DELETE = 'delete'
  const SEND = 'send'
  const ERROR = 'error'
  const SUCCESS = 'success'

  const [images, setImages] = useState([])
  const [selectedImages, setSelectedImages] = useState([])
  const [newImages, setNewImages] = useState(false)
  const [loading, setLoading] = useState(true)
  const [showConfirmationModal, setShowConfirmationModal] = useState(false)
  const [confirmationFunction, setConfirmationFunction] = useState()
  const [confirmationMessage, setConfirmationMessage] = useState()
  const [activeImages, setActiveImages] = useState(false)
  const [showAlertMessage, setShowAlertMessage] = useState(false)
  const [typeofMessage, setTypeofMessage] = useState()

  useEffect(() => {
    let unmounted = false
    const source = axios.CancelToken.source()

    // If this isn't set an error occures because context state is not yet set
    if (context.images) {
      const findImages = context.images.filter(i => i.orderId === orderId)
      if (!unmounted) {
        if (findImages.length > 0) {
          setActiveImages(findImages.some(img => img.submittedToNext === false))
          setImages(findImages)

          if (!newImages) {
            setLoading(false)
          }
        } else {
          axios({
            method: 'GET',
            url: URL.SERVER_GET_IMAGE_URL + orderId,
            headers: {
              Authorization: `Bearer ${getSessionCookie()}`
            },
            cancelToken: source.token
          })
            .then(res => {
              if (res.status === 200) {
                const imgs = res.data.map(data => {
                  if (!data.submittedToNext) {
                    setActiveImages(true)
                  }

                  const file = `data:image/jpeg;base64,${data.thumbnail}`
                  const { thumbnail, imageFile, imageCategoryName, ...imgWithoutThumbnail } = data

                  const img = {
                    id: cuid(),
                    imageFile: file,
                    imageCategoryName: '',
                    ...imgWithoutThumbnail
                  }
                  return img
                })

                // Sort by image sequence
                imgs.sort((a, b) => a.imageSequence - b.imageSequence)

                context.handleImages(imgs)
                setLoading(false)
              }
            })
            .catch(err => {
              // No images on order
              if (!axios.isCancel(err) && err.response.status === 404) {
                setImages([])
                setLoading(false)
              }
            })
        }
      }
    }

    return () => {
      unmounted = true
      return source.cancel()
    }
  }, [context, context.images, orderId])

  const onDrop = useCallback(acceptedFiles => {
    setLoading(true)
    setNewImages(true)
    let highestImageSequence

    if (images.length === 0) {
      highestImageSequence = -1
    } else {
      highestImageSequence = Math.max(...images.map(img => img.imageSequence))
    }

    const axiosCalls = []

    acceptedFiles.map((file, index, arr) => {
      const reader = new window.FileReader()
      reader.onload = e => {
        highestImageSequence += 1

        // Remove data:image/jpeg;base64 from string
        var src = e.target.result.replace(/^data:image\/[a-z]+;base64,/, '')

        const img = new Image(orderId, src, file.name, highestImageSequence)

        // Upload to db on drop
        const promise = axios.post(URL.SERVER_POST_IMAGE_URL, img, {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getSessionCookie()}`
          }
        })

        context.handleImages(prevState => [
          ...prevState,
          {
            id: cuid(),
            imageFile: e.target.result,
            fileName: file.name,
            orderId: orderId,
            imageSequence: highestImageSequence,
            imageDescription: '',
            imageCategoryName: '',
            externalImageReference: img.externalImageReference,
            submittedToNext: false
          }
        ])

        axiosCalls.push(promise)

        if (index === arr.length - 1) {
          axios.all(axiosCalls)
            .then(axios.spread((...responses) => {
              responses.forEach(res => {
                const id = res.data.message

                context.handleImages(prevState => [
                  ...prevState,
                  {
                    externalImageReference: id
                  }
                ])
              })

              setLoading(false)
            }))
            .catch(err => {
              console.log(err)
              setLoading(false)
            })
        }
      }
      reader.readAsDataURL(file)
      return file
    })
  }, [orderId, images])

  const moveImage = (dragIndex, hoverIndex, submittedToNext) => {
    if (submittedToNext) {
      return
    }

    const draggedImage = images[dragIndex]

    setImages(
      update(images, {
        $splice: [[dragIndex, 1], [hoverIndex, 0, draggedImage]]
      })
    )
  }

  /**
   * Called when image is dropped.
   * Update imageSequence
   */
  const onImageDrop = submittedToNext => {
    if (submittedToNext) {
      return
    }

    const arr = images.map((img, index) => {
      return {
        orderId: img.orderId,
        imageSequence: index,
        externalImageReference: img.externalImageReference,
        imageDescription: img.imageDescription,
        imageCategoryName: img.imageCategoryName
      }
    })

    axios({
      method: 'PUT',
      url: URL.SERVER_PUT_SORT_IMAGE_URL_1 + orderId + URL.SERVER_PUT_SORT_IMAGE_URL_2,
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${getSessionCookie()}`
      },
      data: arr
    })
      .then()
      .catch(err => console.log(err.response.data))
  }

  const selectImage = target => {
    const el = window.document.getElementById(target.id)
    el.classList.toggle('selected')

    const img = images.find(i => i.id === target.id)

    if (el.className.includes('selected')) {
      setSelectedImages(prev => [...prev, img])
    } else {
      setSelectedImages(prev => prev.filter(i => i.id !== target.id))
    }
  }

  const handleOrderConfirmation = () => {
    setConfirmationMessage(lang.confirmationSendImages)
    setConfirmationFunction(SEND)
    setShowConfirmationModal(true)
  }

  const handleDeleteConfirmation = () => {
    setConfirmationMessage(lang.confirmationDeleteImages)
    setConfirmationFunction(DELETE)
    setShowConfirmationModal(true)
  }

  const sendOrder = () => {
    setShowConfirmationModal(false)
    setLoading(true)

    axios({
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${getSessionCookie()}`
      },
      url: URL.SERVER_POST_NEXT_URL_1 + orderId + URL.SERVER_POST_NEXT_URL_2
    })
      .then(res => {
        images.forEach(img => {
          img.submittedToNext = true

          context.handleOrders([])
          setActiveImages(false)
          setLoading(false)
        })
      })
      .catch(err => {
        setLoading(false)
        console.log(err.response.data)
      })
  }

  const deleteImages = () => {
    setShowConfirmationModal(false)
    setLoading(true)

    const axiosCalls = []
    const imgsIds = []

    selectedImages.forEach((img, index, arr) => {
      const promise = axios({
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getSessionCookie()}`
        },
        url: URL.SERVER_DELETE_IMAGE_URL + img.externalImageReference
      })

      axiosCalls.push(promise)
      imgsIds.push(img.externalImageReference)

      // If at last index
      if (index === arr.length - 1) {
        axios.all(axiosCalls)
          .then(axios.spread((...responses) => {
            imgsIds.forEach(id => {
              context.handleImages(prev => prev.filter(item => item.externalImageReference !== id))
              setSelectedImages(prev => prev.filter(item => item.externalImageReference !== id))
            })

            setLoading(false)
          }))
          .catch(err => {
            console.log(err)
            setLoading(false)
          })
      }
    })
  }

  const renderLoading = () => {
    return (
      <Container style={{ textAlign: 'center' }}>
        <Spinner className='mt-3' animation='border' />
      </Container>
    )
  }

  /**
   * TODO: This is currenty not in use
   */
  const renderMessage = errorOrSuccess => (
    <FlashMessage duration={5000}>
      <Alert variant={
        (errorOrSuccess === ERROR && 'danger') ||
        (errorOrSuccess === SUCCESS && 'success')
      }
      >
        Placeholder
      </Alert>
    </FlashMessage>
  )

  const renderContent = () => {
    return (
      <>
        {showAlertMessage && renderMessage(typeofMessage)}

        <div style={{ textAlign: 'right' }}>
          <OverlayTrigger
            transition={null}
            placement='left'
            overlay={
              <Tooltip>
                {lang.informationTooltip}
              </Tooltip>
            }
          >
            <i style={{ fontSize: '20px' }} className='fas fa-question-circle' />
          </OverlayTrigger>
        </div>

        <DndProvider backend={HTML5Backend}>
          <ImageList
            images={images}
            moveImage={moveImage}
            onDrop={onDrop}
            accept='image/*'
            onImageDrop={onImageDrop}
            selectImage={selectImage}
            handleDeleteConfirmation={handleDeleteConfirmation}
            imagesSelected={!selectedImages.length > 0}
            handleOrderConfirmation={handleOrderConfirmation}
            activeImages={activeImages}
            setLoading={setLoading}
          />
        </DndProvider>

        <Modal
          show={showConfirmationModal}
          onHide={() => setShowConfirmationModal(false)}
          animation={false}
          dialogClassName='confirmation-modal'
          centered
        >
          <Modal.Header closeButton>
            {confirmationMessage}
          </Modal.Header>
          <Modal.Body style={{ textAlign: 'center' }}>
            <Button
              onClick={(confirmationFunction === DELETE && deleteImages) || (confirmationFunction === SEND && sendOrder)}
              style={{ width: '40%' }}
              variant='danger'
            >
              {lang.yes}
            </Button>
            <Button onClick={() => setShowConfirmationModal(false)} className='ml-3' style={{ width: '40%' }} variant='danger'>
              {lang.cancel}
            </Button>
          </Modal.Body>
        </Modal>
      </>
    )
  }

  return (
    loading
      ? renderLoading()
      : renderContent()
  )
}

export default OrderImages
