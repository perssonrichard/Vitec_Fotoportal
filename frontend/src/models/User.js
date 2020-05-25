class User {
  constructor (
    email,
    firstName,
    lastName,
    password,
    cellPhoneNumber,
    address,
    company,
    orgNr,
    city,
    postalCode,
    postalCodeArea) {
    this.email = email
    this.firstName = firstName
    this.lastName = lastName
    this.hashedPassword = password
    this.cellPhoneNumber = cellPhoneNumber
    this.address = address
    this.company = company
    this.orgNr = orgNr
    this.city = city
    this.postalCode = postalCode
    this.postalCodeArea = postalCodeArea
  }
}

export default User
