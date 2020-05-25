import React, { useEffect } from 'react'
import { useDropzone } from 'react-dropzone'
import Image from './Image'
import Button from 'react-bootstrap/Button'
import lang from '../../language/SWE/ImageList.json'

/**
 * List of images
 */
const ImageList = ({
  images,
  moveImage,
  onDrop,
  accept,
  onImageDrop,
  selectImage,
  handleDeleteConfirmation,
  imagesSelected,
  handleOrderConfirmation,
  activeImages,
  setLoading
}) => {
  const { getRootProps, getInputProps, isDragActive, open } = useDropzone({
    onDrop,
    accept,
    noClick: true,
    noKeyboard: true
  })

  const getClassName = (className, isActive) => {
    if (!isActive) return className
    return `${className} ${className}-active`
  }

  const renderImage = (image, index) => {
    return (
      <Image
        image={image}
        index={index}
        key={`${image.id}-image`}
        moveImage={moveImage}
        onImageDrop={onImageDrop}
        selectImage={selectImage}
        setLoading={setLoading}
      />
    )
  }

  return (
    <div className={getClassName('dropzone', isDragActive)} {...getRootProps()}>
      <input className='dropzone-input' {...getInputProps()} />
      <section style={sectionStyle} className='file-list'>
        {
          images.length !== 0
            ? images.map(renderImage)
            : <p style={{ color: 'grey' }}><br />{lang.dragAndReleaseImages}</p>
        }
      </section>
      <div className='image-buttons'>
        <Button onClick={handleDeleteConfirmation} disabled={imagesSelected} className='mt-2'>
          {lang.deleteImages}
        </Button>
        <Button className='mt-2' type='button' onClick={open}>
          {lang.uploadImages}
        </Button>
        <Button className='mt-2' type='button' disabled={!activeImages} onClick={handleOrderConfirmation}>
          {lang.sendOrder}
        </Button>
      </div>
    </div>
  )
}

const sectionStyle = {
  background: '#e4d4d4',
  border: 'solid black 1px',
  overflow: 'auto',
  justifyContent: 'center'
}

export default ImageList
