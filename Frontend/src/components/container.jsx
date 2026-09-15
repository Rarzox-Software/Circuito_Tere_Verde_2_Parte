export default function Container({ children, className }) {
  return (
    <div className={`px-4 mx-auto xl:max-w-7xl xl:px-0 ${className}`} >
      {children}
    </div >
  )
}