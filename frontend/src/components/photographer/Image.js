import React, { useRef, useState, useEffect, useContext } from 'react'
import Modal from 'react-bootstrap/Modal'
import axios from 'axios'
import URL from '../../models/URL'
import { useDrag, useDrop } from 'react-dnd'
import { Form, FormGroup, Button } from 'react-bootstrap'
import { getSessionCookie } from '../../helpers/CookieHandler'
import GlobalContext from '../../context/GlobalContext'
import lang from '../../language/SWE/Image.json'

/**
 * Image model
 */
const Image = ({ image, index, moveImage, onImageDrop, selectImage, setLoading }) => {
  const context = useContext(GlobalContext)
  const type = 'Image'
  const ref = useRef(null)

  const [description, setDescription] = useState()
  const [showImageModal, setShowImageModal] = useState(false)
  const [showConfirmationModal, setShowConfirmationModal] = useState(false)

  const handleDescription = event => setDescription(event.target.value)

  useEffect(() => {
    setDescription(image.imageDescription)
  }, [image.imageDescription])

  const [, drop] = useDrop({
    // Accept will make sure only these element type can be droppable on this element
    accept: type,
    hover (item) {
      if (image.submittedToNext) {
        return
      }
      if (!ref.current) {
        return
      }

      const dragIndex = item.index
      const hoverIndex = index

      // Don't replace items with themselves
      if (dragIndex === hoverIndex) {
        return
      }

      // Move the content
      moveImage(dragIndex, hoverIndex, image.submittedToNext)
      // Update the index for dragged item directly to avoid flickering when half dragged
      item.index = hoverIndex
    },
    // Called when item is dropped
    drop: () => {
      onImageDrop(image.submittedToNext)
    }
  })

  const [{ isDragging }, drag] = useDrag({
    canDrag: !image.submittedToNext,
    item: { type, id: image.id, index },
    collect: monitor => ({
      isDragging: monitor.isDragging()
    })
  })

  // initialize drag and drop into the element
  drag(drop(ref))

  const saveDescription = event => {
    event.preventDefault()

    const id = image.externalImageReference

    axios({
      method: 'PUT',
      url: URL.SERVER_PUT_IMAGE_URL + id,
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${getSessionCookie()}`
      },
      data: {
        externalImageReference: id,
        imageDescription: description,
        imageCategoryName: image.imageCategoryName,
        imageSequence: image.imageSequence
      }
    })
      .then()
      .catch(err => console.log(err.response.data))

    setShowImageModal(false)
  }

  const confirmationWindow = event => {
    event.preventDefault()
    setShowImageModal(false)
    setShowConfirmationModal(true)
  }

  const deleteImage = () => {
    setShowImageModal(false)
    setLoading(true)
    const id = image.externalImageReference

    axios({
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${getSessionCookie()}`
      },
      url: URL.SERVER_DELETE_IMAGE_URL + id,
      data: id
    })
      .then(() => {
        context.handleImages(img => img.filter(item => item.externalImageReference !== image.externalImageReference))
        setLoading(false)
        setShowConfirmationModal(true)
      })
      .catch(err => {
        console.log(err.response.data)
        setLoading(false)
      })
  }

  const handleClick = event => {
    if (image.submittedToNext) {
      return
    }

    if (event.ctrlKey) {
      selectImage(event.target)
    } else {
      setShowImageModal(true)
    }
  }

  return (
    <div
      ref={ref}
      style={{ opacity: isDragging ? 0 : 1 }}
      className={image.submittedToNext ? 'file-item image-submitted' : 'file-item'}
    >
      <img onClick={handleClick} alt={`img - ${image.id}`} src={image.imageFile} className='file-img' id={image.id} />

      <Modal show={showImageModal} onHide={() => setShowImageModal(false)} animation={false} dialogClassName='img-modal'>
        <Modal.Header closeButton>
          {image.fileName}
        </Modal.Header>
        <Modal.Body>
          <img alt={`img - ${image.id}`} src={image.imageFile} style={imageStyle} />

          <Form onSubmit={saveDescription}>
            <FormGroup>
              <Form.Label className='mt-2'>{lang.descriptionToBroker}</Form.Label>
              <Form.Control style={{ resize: 'none' }} value={description} onChange={handleDescription} as='textarea' rows='3' />
            </FormGroup>
            <Button type='submit' className='float-right' variant='primary'>{lang.save}</Button>
          </Form>
          <Form onSubmit={confirmationWindow}>
            <Button type='submit' variant='primary'>{lang.deleteImage}</Button>
          </Form>

        </Modal.Body>
      </Modal>

      <Modal
        show={showConfirmationModal}
        onHide={() => setShowConfirmationModal(false)}
        animation={false}
        dialogClassName='confirmation-modal'
        centered
      >
        <Modal.Header closeButton>
          {lang.areYouSure}
        </Modal.Header>
        <Modal.Body style={{ textAlign: 'center' }}>
          <Button onClick={deleteImage} style={{ width: '40%' }} variant='danger'>
            {lang.yes}
          </Button>
          <Button onClick={() => setShowConfirmationModal(false)} className='ml-3' style={{ width: '40%' }} variant='danger'>
            {lang.cancel}
          </Button>
        </Modal.Body>
      </Modal>
    </div>
  )
}

const imageStyle = {
  width: '100%',
  height: '100%'
}

export default Image
