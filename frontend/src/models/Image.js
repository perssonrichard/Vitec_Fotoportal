class Image {
  constructor (orderId, imageFile, fileName, imageSequence) {
    this.orderId = orderId
    this.imageFile = imageFile
    this.fileName = fileName
    this.imageSequence = imageSequence
    this.imageDescription = ''
    this.imageCategoryName = ''
    this.externalImageReference = '1'
    this.submittedToNext = false
  }
}

export default Image
